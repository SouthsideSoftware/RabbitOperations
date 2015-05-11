using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metrics;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;
using Microsoft.AspNet.SignalR;
using Polly;
using Raven.Json.Linq;

namespace RabbitOperations.Collector.Service
{
    public class QueuePoller : IQueuePoller
    {
        private readonly CancellationToken cancellationToken;
        private readonly IRabbitConnectionFactory rabbitConnectionFactory;
        private readonly IHeaderParser headerParser;
        private readonly IDocumentStore documentStore;
        private readonly IActiveQueuePollers activeQueuePollers;
        private readonly IStoreMessagesFactory storeMessagesFactory;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Meter messageMeter;
        private int consecutiveRetryCount;

        internal QueuePoller(Guid key, IQueueSettings queueSettings)
        {
            Verify.RequireNotNull(key, "key");
            Verify.RequireNotNull(queueSettings, "queueSettings");

            QueueSettings = queueSettings;
            Key = key;
        }

        public QueuePoller(IQueueSettings queueSettings, CancellationToken cancellationToken, IRabbitConnectionFactory rabbitConnectionFactory,
            IHeaderParser headerParser, IDocumentStore documentStore, IActiveQueuePollers activeQueuePollers, IStoreMessagesFactory storeMessagesFactory)
        {
            Verify.RequireNotNull(queueSettings, "queueSettings");
            Verify.RequireNotNull(cancellationToken, "cancellationToken");
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(rabbitConnectionFactory, "rabbitConnectionFactory");
            Verify.RequireNotNull(activeQueuePollers, "activeQueuePollers");
            Verify.RequireNotNull(storeMessagesFactory, "storeMessagesFactory");

            QueueSettings = queueSettings;
            this.cancellationToken = cancellationToken;
            this.rabbitConnectionFactory = rabbitConnectionFactory;
            this.headerParser = headerParser;
            this.documentStore = documentStore;
            this.activeQueuePollers = activeQueuePollers;
            this.storeMessagesFactory = storeMessagesFactory;
            Key = Guid.NewGuid();

            messageMeter = Metric.Meter(string.Format("RabbitOperations.QueuePoller.Messages.{0}.{1}", QueueSettings.ApplicationId, QueueSettings.QueueName), Unit.Items, TimeUnit.Seconds, tags:new MetricTags("QueuePoller"));
        }

        public IQueueSettings QueueSettings { get; protected set; }

        public Guid Key { get; protected set; }

        public void Poll()
        {
            activeQueuePollers.Add(this);
            //will retry for about 3.5 days
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(10000, retryCount => GetRetryDelay(), (exception, retryDelay, context) =>
                {
                    logger.ErrorException($"Retrying with delay {retryDelay} on {QueueSettings.LogInfo} after exception", exception);
                });
            retryPolicy.Execute(InnerPoll);
            logger.Info("Shutting down queue poller for {0} because of cancellation request", QueueSettings.LogInfo);
            activeQueuePollers.Remove(this);
        }

        /// <summary>
        /// Exponetial backoff of timeout until we hit the sixth retry.
        /// After that, its 2^5 (32) seconds per retry
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetRetryDelay()
        {
            consecutiveRetryCount++;
            if (consecutiveRetryCount <= 5)
            {
                return TimeSpan.FromSeconds(Math.Pow(2, consecutiveRetryCount));
            }
            return TimeSpan.FromSeconds(Math.Pow(2, 5));
        }

        private void InnerPoll()
        {
            if (cancellationToken.IsCancellationRequested) return;
            logger.Info("Started queue poller for {0} with expiration of {1} hours", QueueSettings.LogInfo,
                QueueSettings.DocumentExpirationInHours);
            try
            {
                using (
                    var connection =
                        rabbitConnectionFactory.Create(QueueSettings.RabbitConnectionString,
                            (ushort) QueueSettings.HeartbeatIntervalSeconds).CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.BasicQos(0, 1, false);
                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(QueueSettings.QueueName, false, consumer);
                        logger.Info("Begin polling {0}{1}", QueueSettings.LogInfo,
                            QueueSettings.MaxMessagesPerRun > 0
                                ? string.Format(" to read a maximum of {0} messages", QueueSettings.MaxMessagesPerRun)
                                : "");
                        long messageCount = 0;
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            BasicDeliverEventArgs ea = null;
                            try
                            {
                                YieldToOtherPollers();
                                consumer.Queue.Dequeue(QueueSettings.PollingTimeoutMilliseconds, out ea);
                            }
                            catch (Exception err)
                            {
                                logger.ErrorException("Dequeue error ", err);
                                throw;
                            }
                            logger.Trace("Dequeue completed for {0}{1}", QueueSettings.LogInfo,
                                ea == null ? " without a message (timeout)" : " with a message");
                            if (ea != null)
                            {
                                try
                                {
                                    HandleMessage(new RawMessage(ea));
                                    channel.BasicAck(ea.DeliveryTag, false);
                                    messageCount++;
                                    if (QueueSettings.MaxMessagesPerRun > 0 &&
                                        messageCount >= QueueSettings.MaxMessagesPerRun)
                                    {
                                        break;
                                    }
                                }
                                catch (Exception err)
                                {
                                    channel.BasicNack(ea.DeliveryTag, false, true);
                                    logger.Error("Error on {0} with details {1}", QueueSettings.LogInfo, err);
                                    //todo: move the message out of the way!
                                    //todo: make this a bit more resilient (retries etc.)
                                    throw;
                                }
                            }
                            consecutiveRetryCount = 0;
                        }
                    }
                }
            }
            catch (EndOfStreamException err)
            {
                logger.ErrorException($"Error on {QueueSettings.LogInfo}", err);
                throw;
            }
        }

        private static void YieldToOtherPollers()
        {
            Thread.Sleep(1);
        }

        public void HandleMessage(IRawMessage message)
        {
            logger.Trace("handling message on {0}", QueueSettings.LogInfo);
            storeMessagesFactory.MessageStorageServiceFor(message).Store(message, QueueSettings);
            messageMeter.Mark();
        }
    }
}

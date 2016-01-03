using System;
using System.IO;
using System.Threading;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    //TODO: Bring back the metrics
    public class QueuePoller : IQueuePoller
    {
        private readonly IActiveQueuePollers activeQueuePollers;
        private readonly CancellationToken cancellationToken;
        private readonly IDocumentStore documentStore;
        private readonly IHeaderParser headerParser;
        //private readonly Meter messageMeter;
        private readonly IRabbitConnectionFactory rabbitConnectionFactory;
        private readonly IStoreMessagesFactory storeMessagesFactory;
        private int consecutiveRetryCount;

        internal QueuePoller(Guid key, IQueueSettings queueSettings)
        {
            Verify.RequireNotNull(key, "key");
            Verify.RequireNotNull(queueSettings, "queueSettings");

            QueueSettings = queueSettings;
            Key = key;
        }

        public QueuePoller(IQueueSettings queueSettings, CancellationToken cancellationToken,
            IRabbitConnectionFactory rabbitConnectionFactory,
            IHeaderParser headerParser, IDocumentStore documentStore, IActiveQueuePollers activeQueuePollers,
            IStoreMessagesFactory storeMessagesFactory)
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

            //messageMeter =
            //    Metric.Meter(
            //        string.Format("RabbitOperations.QueuePoller.Messages.{0}.{1}", QueueSettings.ApplicationId,
            //            QueueSettings.QueueName), Unit.Items, TimeUnit.Seconds, tags: new MetricTags("QueuePoller"));
        }

        public IQueueSettings QueueSettings { get; protected set; }

        public Guid Key { get; protected set; }

        public void Poll()
        {
            activeQueuePollers.Add(this);
            //will retry for a little less than 7 days
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(10000, retryCount => GetRetryDelay(),
                    (exception, retryDelay, context) =>
                    {
                        Log.Error(
                            exception, $"Retry #{consecutiveRetryCount} with delay {retryDelay} on {QueueSettings.LogInfo} after exception");
                    });
            retryPolicy.Execute(InnerPoll);
            Log.Information("Shutting down queue poller for {0} because of cancellation request", QueueSettings.LogInfo);
            activeQueuePollers.Remove(this);
        }

        public void HandleMessage(IRawMessage message)
        {
            Log.Verbose("handling message on {0}", QueueSettings.LogInfo);
            storeMessagesFactory.MessageStorageServiceFor(message).Store(message, QueueSettings);
            //messageMeter.Mark();
        }

        /// <summary>
        ///     Exponetial backoff of timeout until we hit the sixth retry.
        ///     After that, its 2^6 (64) seconds per retry
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetRetryDelay()
        {
            consecutiveRetryCount++;
            if (consecutiveRetryCount <= 6)
            {
                return TimeSpan.FromSeconds(Math.Pow(2, consecutiveRetryCount));
            }
            return TimeSpan.FromSeconds(Math.Pow(2, 6));
        }

        private void InnerPoll()
        {
            if (cancellationToken.IsCancellationRequested) return;
            Log.Information(
                "Started queue poller for {0} with audit expiration of {1} hours and error expiration of {2} hours",
                QueueSettings.LogInfo,
                QueueSettings.DocumentExpirationInHours, QueueSettings.ErrorDocumentExpirationInHours);
            try
            {
                using (
                    var connection =
                        rabbitConnectionFactory.Create(QueueSettings.RabbitConnectionString,
                            (ushort) QueueSettings.HeartbeatIntervalSeconds).CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.BasicQos(0, QueueSettings.Prefetch, false);
                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(QueueSettings.QueueName, false, consumer);
                        Log.Information("Begin polling {0}{1}", QueueSettings.LogInfo,
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
                                Log.Error(err, "Dequeue error ");
                                throw;
                            }
                            Log.Verbose("Dequeue completed for {0}{1}", QueueSettings.LogInfo,
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
                                    Log.Error(err, $"Error on {QueueSettings.LogInfo}");
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
                Log.Error(err, $"Error on {QueueSettings.LogInfo}");
                throw;
            }
        }

        private static void YieldToOtherPollers()
        {
            Thread.Sleep(1);
        }
    }
}
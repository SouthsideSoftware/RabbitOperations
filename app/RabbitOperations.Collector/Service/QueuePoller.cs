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
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string queueLogInfo;
        private readonly Meter messageMeter;

        internal QueuePoller(Guid key, IQueueSettings queueSettings)
        {
            Verify.RequireNotNull(key, "key");
            Verify.RequireNotNull(queueSettings, "queueSettings");

            QueueSettings = queueSettings;
            Key = key;
        }

        public QueuePoller(IQueueSettings queueSettings, CancellationToken cancellationToken, IRabbitConnectionFactory rabbitConnectionFactory,
            IHeaderParser headerParser, IDocumentStore documentStore, IActiveQueuePollers activeQueuePollers)
        {
            Verify.RequireNotNull(queueSettings, "queueSettings");
            Verify.RequireNotNull(cancellationToken, "cancellationToken");
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(rabbitConnectionFactory, "rabbitConnectionFactory");
            Verify.RequireNotNull(activeQueuePollers, "activeQueuePollers");

            QueueSettings = queueSettings;
            this.cancellationToken = cancellationToken;
            this.rabbitConnectionFactory = rabbitConnectionFactory;
            this.headerParser = headerParser;
            this.documentStore = documentStore;
            this.activeQueuePollers = activeQueuePollers;
            queueLogInfo = string.Format("queue {0} in environment {1}({2})", QueueSettings.QueueName,
                QueueSettings.ApplicationName, QueueSettings.ApplicationId);
            Key = Guid.NewGuid();

            messageMeter = Metric.Meter(string.Format("RabbitOperations.QueuePoller.Messages.{0}.{1}", QueueSettings.ApplicationId, QueueSettings.QueueName), Unit.Items, tags:new MetricTags("QueuePoller"));
        }

        public IQueueSettings QueueSettings { get; protected set; }

        public Guid Key { get; protected set; }

        public void Poll()
        {
            activeQueuePollers.Add(this);
            logger.Info("Started queue poller for {0} with expiration of {1} hours", queueLogInfo, QueueSettings.DocumentExpirationInHours);
            using (var connection = rabbitConnectionFactory.Create(QueueSettings).CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(QueueSettings.QueueName, false, consumer);
                    logger.Info("Begin polling {0}{1}", queueLogInfo,
                        QueueSettings.MaxMessagesPerRun > 0
                            ? string.Format(" to read a maximum of {0} messages", QueueSettings.MaxMessagesPerRun)
                            : "");
                    long messageCount = 0;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        BasicDeliverEventArgs ea = null;
                        consumer.Queue.Dequeue(QueueSettings.PollingTimeoutMilliseconds, out ea);
                        logger.Trace("Dequeue completed for {0}{1}", queueLogInfo,
                            ea == null ? " without a message (timeout)" : " with a message");
                        if (ea != null)
                        {
                            try
                            {
                                HandleMessage(new RawMessage(ea));
                                channel.BasicAck(ea.DeliveryTag, false);
                                messageCount++;
                                if (QueueSettings.MaxMessagesPerRun > 0 && messageCount >= QueueSettings.MaxMessagesPerRun)
                                {
                                    break;
                                }
                            }
                            catch (Exception err)
                            {
                                channel.BasicNack(ea.DeliveryTag, false, true);
                                logger.Error("Error on {0} with details {1}", queueLogInfo, err);
                                //todo: move the message out of the way!
                                //todo: make this a bit more resilient (retries etc.)
                                throw;
                            }
                        }
                    }
                }
            }
            logger.Info("Shutting down queue poller for {0} because of cancellation request", queueLogInfo);
            activeQueuePollers.Remove(this);
        }

 

        public void HandleMessage(IRawMessage message)
        {
            logger.Trace("handling message on {0}", queueLogInfo);

            var document = new MessageDocument
            {
                ApplicationId = QueueSettings.ApplicationId
            };
            headerParser.AddHeaderInformation(message, document);
            document.Body = message.Body;
            var expiry = DateTime.UtcNow.AddHours(QueueSettings.DocumentExpirationInHours);

            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                session.Store(document);
                session.Advanced.GetMetadataFor(document)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                session.SaveChanges();
                logger.Trace("Saved document for message with id {0} from {1}", document.Id, queueLogInfo);
            }
            messageMeter.Mark();
        }
    }
}

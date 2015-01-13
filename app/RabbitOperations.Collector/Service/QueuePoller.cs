using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class QueuePoller : IQueuePoller
    {
        private readonly CancellationToken cancellationToken;
        private readonly IConnectionFactory connectionFactory;
        private readonly IHeaderParser headerParser;
        private readonly IDocumentStore documentStore;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public QueuePoller(string queueName, CancellationToken cancellationToken, IConnectionFactory connectionFactory, IHeaderParser headerParser, IDocumentStore documentStore)
        {
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireNotNull(cancellationToken, "cancellationToken");
            Verify.RequireNotNull(connectionFactory, "connectionFactory");
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");

            this.QueueName = queueName;
            this.cancellationToken = cancellationToken;
            this.connectionFactory = connectionFactory;
            this.headerParser = headerParser;
            this.documentStore = documentStore;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }

        public string QueueName { get; private set; }

        public void Poll()
        {
            logger.Info("Started queue poller for {0}", QueueName);
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.BasicQos(0, 1, false); 
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(QueueName, false, consumer);
                    logger.Info("Begin polling {0}", QueueName);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        BasicDeliverEventArgs ea = null;
                        consumer.Queue.Dequeue(5000, out ea);
                        logger.Trace("Dequeue completed {0}", ea == null ? "without a message (timeout)" : "with a message");
                        if (ea != null)
                        {
                            try
                            {
                                HandleMessage(new RawMessage(ea));
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                            catch (Exception err)
                            {
                                logger.Error(err);
                                //todo: hmmmm.... This could be a real problem.  It will keep putting this message back on the queue and if there is something wrong that we cannot parse it, yuck.
                                channel.BasicNack(ea.DeliveryTag, false, true);
                            }
                        }
                    }
                }
            }
            logger.Info("Shutting down queue poller for {0} because of cancellation request", QueueName);
        }

        public void HandleMessage(IRawMessage message)
        {
            logger.Trace("handling message");
            var document = new MessageDocument();
            headerParser.AddHeaderInformation(message, document);
            document.Body = message.Body;
            using (var session = documentStore.OpenSession())
            {
                session.Store(document);
                session.SaveChanges();
                logger.Trace("Saved document for message with id {0}", document.Id);
            }
        }
    }
}

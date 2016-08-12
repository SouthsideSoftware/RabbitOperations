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
    public class ApplicationListener : IApplicationListener
    {
        private readonly CancellationToken cancellationToken;
        private readonly IRabbitConnectionFactory rabbitConnectionFactory;
        private readonly IHeaderParser headerParser;
        private readonly IDocumentStore documentStore;
        private readonly IActiveApplicationListeners activeApplicationListeners;
        private readonly IStoreMessagesFactory storeMessagesFactory;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Meter auditMeeter;
		private readonly Meter errorMeter;
        private int consecutiveRetryCount;
	    private bool shutdownListener;

        internal ApplicationListener(Guid key, IApplicationConfiguration applicationConfiguration)
        {
            Verify.RequireNotNull(key, "key");
            Verify.RequireNotNull(applicationConfiguration, "applicationConfiguration");

            ApplicationConfiguration = applicationConfiguration;
            Key = key;
        }

        public ApplicationListener(IApplicationConfiguration applicationConfiguration, CancellationToken cancellationToken, IRabbitConnectionFactory rabbitConnectionFactory,
            IHeaderParser headerParser, IDocumentStore documentStore, IActiveApplicationListeners activeApplicationListeners, IStoreMessagesFactory storeMessagesFactory)
        {
            Verify.RequireNotNull(applicationConfiguration, "applicationConfiguration");
            Verify.RequireNotNull(cancellationToken, "cancellationToken");
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(rabbitConnectionFactory, "rabbitConnectionFactory");
            Verify.RequireNotNull(activeApplicationListeners, "activeApplicationListeners");
            Verify.RequireNotNull(storeMessagesFactory, "storeMessagesFactory");

			ApplicationConfiguration = applicationConfiguration;
            this.cancellationToken = cancellationToken;
            this.rabbitConnectionFactory = rabbitConnectionFactory;
            this.headerParser = headerParser;
            this.documentStore = documentStore;
            this.activeApplicationListeners = activeApplicationListeners;
            this.storeMessagesFactory = storeMessagesFactory;
            Key = Guid.NewGuid();

            auditMeeter = Metric.Meter(string.Format("RabbitOperations.ApplicationPoller.Messages.{0}.{1}", ApplicationConfiguration.ApplicationId, ApplicationConfiguration.AuditQueue), Unit.Items, TimeUnit.Seconds, tags:new MetricTags("ApplicationPoller"));
			errorMeter = Metric.Meter(string.Format("RabbitOperations.ApplicationPoller.Messages.{0}.{1}", ApplicationConfiguration.ApplicationId, ApplicationConfiguration.ErrorQueue), Unit.Items, TimeUnit.Seconds, tags: new MetricTags("ApplicationPoller"));
		}

	    public IApplicationConfiguration ApplicationConfiguration { get; }

	    public void Start()
	    {
			Listen();
		}

	    public void Stop()
	    {
		    throw new NotImplementedException();
	    }

        public Guid Key { get; protected set; }

        /// <summary>
        /// Exponetial backoff of timeout until we hit the sixth retry.
        /// After that, its 2^6 (64) seconds per retry
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

	    private void Listen()
	    {
			if (cancellationToken.IsCancellationRequested) return;
			logger.Info("Started application listener for {0} with audit expiration of {1} hours and error expiration of {2} hours", QueueSettings.LogInfo,
				QueueSettings.DocumentExpirationInHours, QueueSettings.ErrorDocumentExpirationInHours);
			try
			{
				using (
					var connection =
						rabbitConnectionFactory.Create(QueueSettings.RabbitConnectionString,
							(ushort)QueueSettings.HeartbeatIntervalSeconds).CreateConnection())
				{
					using (var channel = connection.CreateModel())
					{
						channel.BasicQos(0, QueueSettings.Prefetch, false);
						long messageCount = 0;
						var consumer = new EventingBasicConsumer(channel);
						consumer.Received += (model, ea) =>
						{
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
										shutdownListener = true;
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
						};
						channel.BasicConsume(queue: QueueSettings.QueueName, noAck: false, consumer: consumer);
						while (!cancellationToken.IsCancellationRequested && !shutdownListener)
						{
							Thread.Sleep(1000);
						}
					}
				}
			}
			catch (EndOfStreamException err)
			{
				logger.Error(err, $"Error on {QueueSettings.LogInfo}");
				throw;
			}
		}


        private void InnerPoll()
        {
            if (cancellationToken.IsCancellationRequested) return;
            logger.Info("Started queue poller for {0} with audit expiration of {1} hours and error expiration of {2} hours", QueueSettings.LogInfo,
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
#pragma warning disable 618
                        var consumer = new QueueingBasicConsumer(channel);
#pragma warning restore 618
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
                                logger.Error(err, "Dequeue error ");
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
                logger.Error(err, $"Error on {QueueSettings.LogInfo}");
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

using Metrics;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Polly;
using RabbitOperations.Collector.RavenDB;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Service
{
	public class ApplicationListener : IApplicationListener
	{
		private readonly IRabbitConnectionFactory rabbitConnectionFactory;
		private readonly IHeaderParser headerParser;
		private readonly IDocumentStore documentStore;
		private readonly IActiveApplicationListeners activeApplicationListeners;
		private readonly IStoreMessagesFactory storeMessagesFactory;
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		private int consecutiveRetryCount;
		private bool shutdownListener;
		private long messageCount = 0;
		private Exception rabbitException;
        private Task task;

		internal ApplicationListener(Guid key, IApplicationConfiguration applicationConfiguration)
		{
			Verify.RequireNotNull(key, "key");
			Verify.RequireNotNull(applicationConfiguration, "applicationConfiguration");

			ApplicationConfiguration = applicationConfiguration;
			Key = key;
		}

		public ApplicationListener(IApplicationConfiguration applicationConfiguration, IRabbitConnectionFactory rabbitConnectionFactory,
			IHeaderParser headerParser, IDocumentStore documentStore, IActiveApplicationListeners activeApplicationListeners, IStoreMessagesFactory storeMessagesFactory)
		{
			Verify.RequireNotNull(applicationConfiguration, "applicationConfiguration");
			Verify.RequireNotNull(headerParser, "headerParser");
			Verify.RequireNotNull(documentStore, "documentStore");
			Verify.RequireNotNull(rabbitConnectionFactory, "rabbitConnectionFactory");
			Verify.RequireNotNull(activeApplicationListeners, "activeApplicationListeners");
			Verify.RequireNotNull(storeMessagesFactory, "storeMessagesFactory");

			ApplicationConfiguration = applicationConfiguration;
			this.rabbitConnectionFactory = rabbitConnectionFactory;
			this.headerParser = headerParser;
			this.documentStore = documentStore;
			this.activeApplicationListeners = activeApplicationListeners;
			this.storeMessagesFactory = storeMessagesFactory;
			Key = Guid.NewGuid();
		}

		public IApplicationConfiguration ApplicationConfiguration { get; }

		public void Start()
		{
            logger.Info($"Starting listener for {ApplicationConfiguration.ApplicationLogInfo}");
            task = Task.Factory.StartNew(() =>
            {
                try
                {
                    Listen();
                }
                catch (Exception err)
                {
                    logger.Error(err, $"Application {ApplicationConfiguration.ApplicationLogInfo} failed");
                    throw;
                }
            }, TaskCreationOptions.LongRunning);
		}
		public void Stop()
		{
            logger.Info($"Stopping listener for {ApplicationConfiguration.ApplicationLogInfo}");
            shutdownListener = true;
            try
            {
                task.Wait();
                logger.Info($"Stopped listener for {ApplicationConfiguration.ApplicationLogInfo}");
            }
            catch (Exception ex)
            {
                logger.Error($"Application listener for {ApplicationConfiguration.ApplicationLogInfo} encountered exception while shutting down");

                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    logger.Error(aggregateException.Flatten());
                }
                else
                {
                    logger.Error(ex.Message);
                }

                logger.Error(ex.StackTrace);
            }
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
				return TimeSpan.FromSeconds(Math.Pow(2, consecutiveRetryCount));
			return TimeSpan.FromSeconds(Math.Pow(2, 6));
		}

		private void Listen()
		{
			//will retry for a little less than 7 days
			var retryPolicy = Policy
				.Handle<Exception>()
				.WaitAndRetry(10000, retryCount => GetRetryDelay(), (exception, retryDelay, context) =>
				{
					logger.Error(exception, $"Retry #{consecutiveRetryCount} with delay {retryDelay} on {ApplicationConfiguration.ApplicationLogInfo} after exception");
				});
			retryPolicy.Execute(InnerListen);
		}

		private void InnerListen()
		{
			messageCount = 0;
			rabbitException = null;
			if (shutdownListener) return;
			logger.Info($"Started application listener for {ApplicationConfiguration.ApplicationLogInfo} with audit expiration of {ApplicationConfiguration.DocumentExpirationInHours} hours " + 
				"and error expiration of {ApplicationConfiguration.ErrorDocumentExpirationInHours} hours");
			try
			{
				using (
					var connection =
						rabbitConnectionFactory.Create(ApplicationConfiguration.RabbitConnectionString,
							(ushort)ApplicationConfiguration.HeartbeatIntervalSeconds).CreateConnection())
				using (var channel = connection.CreateModel())
				{
					channel.BasicQos(0, ApplicationConfiguration.Prefetch, false);
					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (ch, ea) =>
					{
						if (ea != null)
							try
							{
								using (var session = documentStore.OpenSessionForDefaultTenant())
								{
									logger.Trace($"store message for {ApplicationConfiguration.ApplicationLogInfo}");
									session.Store(new RawMessage(ea));
									session.SaveChanges();
									//todo: publish message for further processing
								}
								channel.BasicAck(ea.DeliveryTag, false);
								var localMessageCount = Interlocked.Increment(ref messageCount);
								if (ApplicationConfiguration.MaxMessagesPerRun > 0 &&
									localMessageCount >= ApplicationConfiguration.MaxMessagesPerRun)
									shutdownListener = true;
							}
							catch (Exception err)
							{
								channel.BasicNack(ea.DeliveryTag, false, true);
								logger.Error(err, $"Error reading message for {ApplicationConfiguration.ApplicationLogInfo} originally delivered to {ea.Exchange}");
									//todo: move the message out of the way!
									//todo: make this a bit more resilient (retries etc.)
									throw;
							}
						consecutiveRetryCount = 0;
					};
					consumer.Shutdown += (sender, e) =>
					{
						if (!shutdownListener)
						{
							logger.Error($"Experienced shutdown on basic consumer for application {ApplicationConfiguration.ApplicationLogInfo}. Retrying.");
							rabbitException = new Exception("Unexpected shutdown of Rabbit basic consumer");
						}
					};
					channel.BasicConsume(queue: ApplicationConfiguration.AuditQueue, noAck: false, consumer: consumer);
					channel.BasicConsume(queue: ApplicationConfiguration.ErrorQueue, noAck: false, consumer: consumer);
					while (!shutdownListener && rabbitException == null)
					{
						Thread.Sleep(5000);
					}
					if (rabbitException != null)
					{
						throw rabbitException;
					}
				}
			}
			catch (EndOfStreamException err)
			{
				logger.Error(err, $"Error on application listener for {ApplicationConfiguration.ApplicationLogInfo}");
				throw;
			}
		}
	}
}

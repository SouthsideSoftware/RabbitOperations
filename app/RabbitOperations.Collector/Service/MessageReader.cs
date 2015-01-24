using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Service
{
    public class MessageReader : IMessageReader
    {
        private readonly ICancellationTokenSource cancellationTokenSource;
        private readonly ISettings settings;
        private readonly IQueuePollerFactory queuePollerFactory;
        private CancellationToken cancellationToken;
        private IList<Task> queuePollers = new List<Task>();
        private Logger logger = LogManager.GetCurrentClassLogger();

        public MessageReader(ICancellationTokenSource cancellationTokenSource, ISettings settings, IQueuePollerFactory queuePollerFactory)
        {
            Verify.RequireNotNull(cancellationTokenSource, "cancellationTokenSource");
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(queuePollerFactory, "queuePollerFactory");

            this.cancellationTokenSource = cancellationTokenSource;
            this.settings = settings;
            this.queuePollerFactory = queuePollerFactory;

            cancellationToken = cancellationTokenSource.Token;
        }

        public void Start()
        {
            logger.Info("Collector starting...");
            foreach (var environment in settings.Environments)
            {
                StartPollingQueue(new QueueSettings(environment.AuditQueue, environment));
                StartPollingQueue(new QueueSettings(environment.ErrorQueue, environment));
            }
            logger.Info("Collector started");
        }

        private void StartPollingQueue(IQueueSettings queueSettings)
        {
            queuePollers.Add(Task.Factory.StartNew(() =>
            {
                string queueLogInfo = string.Format("queue {0} in environment {1}({2})", queueSettings.QueueName, queueSettings.EnvironmentName, queueSettings.EnvironmentId);
                try
                {
                    var queuePoller = queuePollerFactory.Create(queueSettings, cancellationToken);
                    queuePoller.Poll();
                }
                catch (Exception err)
                {
                    logger.Error("Failed in queue poller for {0} with error {1}", queueLogInfo, err);
                    throw;
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
        }

        public void Stop()
        {
            logger.Info("Collector stopping...");
            cancellationTokenSource.Cancel();
            HandleShutdown();
            logger.Info("Collector stopped");
        }

        private void HandleShutdown()
        {
            try
            {
                Task.WaitAll(queuePollers.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("Collector encountered exception while shutting down");

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
    }
}
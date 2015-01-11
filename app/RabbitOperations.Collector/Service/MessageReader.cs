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
            StartPollingQueue(settings.AuditQueue);
            StartPollingQueue(settings.ErrorQueue);
            logger.Info("Collector started");
        }

        private void StartPollingQueue(string queueName)
        {
            queuePollers.Add(Task.Factory.StartNew(() =>
            {
                var queuePoller = queuePollerFactory.Create(queueName, cancellationToken);
                queuePoller.Poll();
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
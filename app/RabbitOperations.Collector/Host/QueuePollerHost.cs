using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Host.Interfaces;
using RabbitOperations.Collector.RavenDB.Interfaces;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain.Configuration;
using Raven.Client;
using Raven.Storage.Esent.SchemaUpdates;
using SouthsideUtility.Core.DesignByContract;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Host
{
    public class QueuePollerHost : IQueuePollerHost
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private IList<Task> queuePollers = new List<Task>();
        private readonly IQueuePollerFactory queuePollerFactory;
        private readonly IDocumentStore documentStore;
        private readonly ISchemaUpdater schemaUpdater;
        private readonly ICancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private readonly ISettings settings;

        public QueuePollerHost(ICancellationTokenSource cancellationTokenSource, ISettings settings, IQueuePollerFactory queuePollerFactory, IDocumentStore documentStore, ISchemaUpdater schemaUpdater)
        {
            Verify.RequireNotNull(cancellationTokenSource, "cancellationTokenSource");
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(queuePollerFactory, "queuePollerFactory");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(schemaUpdater, "schemaUpdater");

            this.cancellationTokenSource = cancellationTokenSource;
            this.settings = settings;
            this.queuePollerFactory = queuePollerFactory;
            this.documentStore = documentStore;
            this.schemaUpdater = schemaUpdater;

            cancellationToken = cancellationTokenSource.Token;
            CreateDefaultEnvirtonmentIfNoneExists();
            UpdateSchemaIfNeeded();
        }

        private void UpdateSchemaIfNeeded()
        {
            schemaUpdater.UpdateSchema();
        }

        public void Start()
        {
            logger.Info("Queue poller host starting...");
            foreach (var environment in settings.Environments)
            {
                if (environment.AutoStartQueuePolling)
                {
                    StartPollingQueue(new QueueSettings(environment.AuditQueue, environment));
                    StartPollingQueue(new QueueSettings(environment.ErrorQueue, environment));
                }
                else
                {
                    logger.Info("Polling for environment {0}({1}) is disabled. This is configured in the web application.", environment.EnvironmentName, environment.EnvironmentId);
                }
            }
            logger.Info("Queue poller host started");
        }

        private void CreateDefaultEnvirtonmentIfNoneExists()
        {
            if (settings.Environments.Count == 0)
            {
                logger.Info("Creating default environment.  Open RavenDB management studio and edit the configuraiton document to setup queue polling");
                settings.Environments.Add(new EnvironmentConfiguration
                {
                    EnvironmentId = "default",
                    EnvironmentName = "Default",
                    AuditQueue = "audit",
                    ErrorQueue = "error",
                    AutoStartQueuePolling = false,
                    RabbitConnectionString = "amqp://[user]:[password]@localhost[/vhost]"
                });
                settings.Save();
            }
        }

        public void Stop()
        {
            logger.Info("Queue poller host stopping...");
            cancellationTokenSource.Cancel();
            HandleShutdown();
            logger.Info("Queue poller host stopped");
        }

        private void StartPollingQueue(IQueueSettings queueSettings)
        {
            queuePollers.Add(Task.Factory.StartNew(() =>
            {
                string queueLogInfo = string.Format("queue {0} in environment {1}({2})", queueSettings.QueueName,
                    queueSettings.EnvironmentName, queueSettings.EnvironmentId);
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

        private void HandleShutdown()
        {
            try
            {
                Task.WaitAll(queuePollers.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("QueuePollerHost encountered exception while shutting down");

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
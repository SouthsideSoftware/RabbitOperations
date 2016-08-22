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
using SouthsideUtility.Core.DesignByContract;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Host
{
    public class ApplicationListenerHost : IApplicationListenerHost
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private IDictionary<string, IApplicationListener> applicationListeners = new Dictionary<string, IApplicationListener>();
        private readonly IApplicationListenerFactory applicationListenerFactory;
        private readonly IDocumentStore documentStore;
        private readonly ISchemaUpdater schemaUpdater;
        private readonly ISettings settings;

        public ApplicationListenerHost(ISettings settings, IApplicationListenerFactory applicationListenerFactory, IDocumentStore documentStore, ISchemaUpdater schemaUpdater)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(applicationListenerFactory, "queuePollerFactory");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(schemaUpdater, "SchemaUpdater");

            this.settings = settings;
            this.applicationListenerFactory = applicationListenerFactory;
            this.documentStore = documentStore;
            this.schemaUpdater = schemaUpdater;

            CreateDefaultEnvirtonmentIfNoneExists();
            UpdateSchemaIfNeeded();
        }

        private void UpdateSchemaIfNeeded()
        {
            schemaUpdater.UpdateSchema();
        }

        public void Start()
        {
            logger.Info("Application listener host starting...");
            foreach (var application in settings.Applications)
            {
                if (application.AutoStartQueuePolling)
                {
                    StartApplicationListener(application);
                }
                else
                {
                    logger.Info($"Polling for application {application.ApplicationLogInfo} is disabled. This is configured in the web application.");
                }
            }
            logger.Info("Application listener host started");
        }

        private void CreateDefaultEnvirtonmentIfNoneExists()
        {
            if (settings.Applications.Count == 0)
            {
                logger.Info("Creating default application.  Open RavenDB management studio and edit the configuration document to setup queue polling");
                settings.Applications.Add(new ApplicationConfiguration
                {
                    ApplicationId = "default",
                    ApplicationName = "Default",
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
            logger.Info("Application listener host stopping...");
            foreach (var applicationListener in applicationListeners.Values)
            {
                applicationListener.Stop();
            }
            logger.Info("Application listener host stopped");
        }

        private void StartApplicationListener(IApplicationConfiguration application)
        {
            var applicationListener = applicationListenerFactory.Create(application);
            applicationListeners.Add(application.ApplicationId, applicationListener);
            applicationListener.Start();
        }
    }
}
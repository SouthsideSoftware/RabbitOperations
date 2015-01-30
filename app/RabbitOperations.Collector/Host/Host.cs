using System;
using System.Net;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Host.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Host
{
    public class Host : IHost
    {
        private readonly string hostName;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettings settings;
        private readonly ISubHostFactory subHostFactory;
        private IQueuePollerHost queuePollerHost;
        private IWebHost webHost;

        public Host(ISubHostFactory subHostFactory, ISettings settings)
        {
            Verify.RequireNotNull(subHostFactory, "subHostFactory");
            Verify.RequireNotNull(settings, "settings");

            this.subHostFactory = subHostFactory;
            this.settings = settings;

            try
            {
                hostName = Dns.GetHostName();
            }
            catch (Exception err)
            {
                logger.Error("Error getting host name {0}", err);
                hostName = "127.0.0.1";
            }
        }

        public void Start()
        {
            logger.Info("Collector starting...");

            queuePollerHost = subHostFactory.CreatePollerHost();
            if (settings.AutoStartQueuePolling)
            {
                queuePollerHost.Start();
            }
            else
            {
                logger.Info("Polling of queues is not set for autostart. See the web application to start manually or to change this setting");
            }

            webHost = subHostFactory.CreateWebHost();
            webHost.Start();

            logger.Info("Collector started.");
            if (settings.EmbedRavenDB)
            {
                logger.Info(
                    "Embedded RavenDB server started.  You can access the RavenDB management studio at http://{0}:{1} or http://localhost:{1}",
                    hostName, settings.EmbeddedRavenDBManagementPort);
            }
        }

        public void Stop()
        {
            logger.Info("Collector stopping...");
            queuePollerHost.Stop();
            webHost.Stop();
            logger.Info("Collector stopped");
        }
    }
}
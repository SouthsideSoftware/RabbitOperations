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
        private IApplicationListenerHost applicationListenerHost;
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
                logger.Error(err, "Error getting host name {0}");
                hostName = "127.0.0.1";
            }
        }

        public void Start()
        {
            logger.Info("Collector starting...");

            applicationListenerHost = subHostFactory.CreatePollerHost();
            if (settings.AutoStartQueuePolling && !settings.SuppressPolling)
            {
                applicationListenerHost.Start();
            }
            else
            {
	            if (settings.SuppressPolling)
	            {
					logger.Info(
						"Polling of queues is suppressed because SuppressPolling is set to true in the configuration file. Change the configuration file setting and restart if you want to poll queues.");
				}
	            else
	            {
		            logger.Info(
			            "Polling of queues is not set for autostart. See the web application to start manually or to change this setting");
	            }
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
            applicationListenerHost.Stop();
            webHost.Stop();
            logger.Info("Collector stopped");
        }
    }
}
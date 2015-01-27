using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Host.Interfaces;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Collector.Web;
using Raven.Database.Server;
using SouthsideUtility.Core.DesignByContract;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Host
{
    public class Host : IHost
    {
        private readonly ISubHostFactory subHostFactory;
        private readonly ISettings settings;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private IQueuePollerHost queuePollerHost;
        private IWebHost webHost;
        private string hostName;

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
            queuePollerHost.Start();

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
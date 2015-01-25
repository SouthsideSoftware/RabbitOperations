using System;
using System.Collections.Generic;
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
        private Logger logger = LogManager.GetCurrentClassLogger();
        private IQueuePollerHost queuePollerHost;
        private IWebHost webHost;

        public Host(ISubHostFactory subHostFactory)
        {
            Verify.RequireNotNull(subHostFactory, "subHostFactory");
            this.subHostFactory = subHostFactory;
        }

        public void Start()
        {
            logger.Info("Collector starting...");

            queuePollerHost = subHostFactory.CreatePollerHost();
            queuePollerHost.Start();

            webHost = subHostFactory.CreateWebHost();
            webHost.Start();

            logger.Info("Collector started");
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
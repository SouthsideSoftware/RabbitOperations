using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Host.Interfaces;
using RabbitOperations.Collector.Web;
using Raven.Database.Server;
using SouthsideUtility.Core.DesignByContract;
using SouthsideUtility.Core.TestableSystem.Interfaces;

namespace RabbitOperations.Collector.Host
{
    public class WebHost : IWebHost
    {
        private readonly ICancellationTokenSource cancellationTokenSource;
        private readonly ISettings settings;
        private CancellationToken cancellationToken;
        private Task webServer;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public WebHost(ICancellationTokenSource cancellationTokenSource, ISettings settings)
        {
            Verify.RequireNotNull(cancellationTokenSource, "cancellationTokenSource");
            Verify.RequireNotNull(settings, "settings");

            this.cancellationTokenSource = cancellationTokenSource;
            this.settings = settings;

            cancellationToken = cancellationTokenSource.Token;
        }
        public void Start()
        {
            logger.Info("Web host starting...");
            StartWebServer();
            logger.Info("Web host started.");
        }

        public void Stop()
        {
            logger.Info("Web host stopping...");
            cancellationTokenSource.Cancel();
            HandleShutdown();
            logger.Info("Web host stopped");
        }

        private void StartWebServer()
        {
            webServer = Task.Factory.StartNew(() =>
            {
                var url = "http://+:8082";
                NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8082);

                using (WebApp.Start<Startup>(url))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void HandleShutdown()
        {
            try
            {
                Task.WaitAll(webServer);
            }
            catch (Exception ex)
            {
                logger.Error("WebHost encountered exception while shutting down");

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Castle.Windsor.Diagnostics.Extensions;
using Metrics.Logging;
using NLog;
using RabbitOperations.Collector.Poller.Interfaces;
using Raven.Database.Extensions;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Poller
{
    public class ApplicationListener : IApplicationListener
    {
        private NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private IList<PollerTaskInfo> runningPollerTasks;
        private IList<IPoller> pollers;

        public ApplicationListener(string name, IList<IPoller> pollersToStart = null)
        {
            Name = name;
            this.pollers = pollersToStart != null ? new List<IPoller>(pollersToStart) : new List<IPoller>();
            runningPollerTasks = new List<PollerTaskInfo>();
        }

        public void Start()
        {
            foreach (var poller in pollers)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var pollerToStart = poller;
                var task = new Task(() =>
                {
                    logger.Info("{0} starting polling", LoggingInfo);
                    try
                    {
                        pollerToStart.Poll();
                        logger.Info("{0} stopping polling", LoggingInfo);
                    }
                    catch (Exception err)
                    {
                        logger.Error("Failed in {0} with error {1}", LoggingInfo, err);
                    }
                    StoppedPolling(pollerToStart);
                }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                runningPollerTasks.Add(new PollerTaskInfo(task, poller, cancellationTokenSource));

                task.Start();
            }
        }

        private void StoppedPolling(IPoller pollerToStart)
        {
            var pollerTaskInfo = runningPollerTasks.FirstOrDefault(x => x.Poller == pollerToStart);
            if (pollerTaskInfo != null)
            {
                runningPollerTasks.Remove(pollerTaskInfo);
            }

        }

        public void Stop()
        {
            logger.Info("{0} stopping...", LoggingInfo);
            foreach (var cancellationTokenSource in runningPollerTasks.Select(x => x.CancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
            }
            HandleShutdown();
            logger.Info("{0} stopped", LoggingInfo);
        }

        protected virtual string LoggingInfo { get { return string.Format("Poller Host {0}", Name); } }

        public string Name { get; private set; }

        private void HandleShutdown()
        {
            try
            {
                Task.WaitAll(runningPollerTasks.Select(x => x.Task).ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("{0} encountered exception while shutting down", LoggingInfo);

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

    public class PollerTaskInfo
    {
        public PollerTaskInfo(Task task, IPoller poller, CancellationTokenSource cancellationTokenSource)
        {
            Verify.RequireNotNull(task, "task");
            Verify.RequireNotNull(poller, "Poller");
            Verify.RequireNotNull(cancellationTokenSource, "cancellationTokenSource");

            Task = task;
            Poller = poller;
            CancellationTokenSource = cancellationTokenSource;
        }
        public Task Task { get; private set; }
        public IPoller Poller { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class ActiveQueuePollers : IActiveQueuePollers
    {
        private readonly ConcurrentDictionary<Guid, IApplicationPoller> activeQueuePollers = new ConcurrentDictionary<Guid, IApplicationPoller>(); 

        public IReadOnlyCollection<IApplicationPoller> ActivePollers
        {
            get { return activeQueuePollers.Values.OrderBy(x => x.QueueSettings.ApplicationId).ThenBy(x => x.QueueSettings.IsErrorQueue).ToList().AsReadOnly(); }
        }

        public void Add(IApplicationPoller applicationPoller)
        {
            Verify.RequireNotNull(applicationPoller, "queuePoller");

            activeQueuePollers.TryAdd(applicationPoller.Key, applicationPoller);
        }

        public void Remove(IApplicationPoller applicationPoller)
        {
            Verify.RequireNotNull(applicationPoller, "queuePoller");

            activeQueuePollers.TryRemove(applicationPoller.Key, out applicationPoller);
        }
    }
}
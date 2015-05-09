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
        private readonly ConcurrentDictionary<Guid, IQueuePoller> activeQueuePollers = new ConcurrentDictionary<Guid, IQueuePoller>(); 

        public IReadOnlyCollection<IQueuePoller> ActivePollers
        {
            get { return activeQueuePollers.Values.OrderBy(x => x.QueueSettings.ApplicationId).ThenBy(x => x.QueueSettings.IsErrorQueue).ToList().AsReadOnly(); }
        }

        public void Add(IQueuePoller queuePoller)
        {
            Verify.RequireNotNull(queuePoller, "queuePoller");

            activeQueuePollers.TryAdd(queuePoller.Key, queuePoller);
        }

        public void Remove(IQueuePoller queuePoller)
        {
            Verify.RequireNotNull(queuePoller, "queuePoller");

            activeQueuePollers.TryRemove(queuePoller.Key, out queuePoller);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class ActiveApplicationListeners : IActiveApplicationListeners
    {
        private readonly ConcurrentDictionary<Guid, IApplicationListener> activeQueuePollers = new ConcurrentDictionary<Guid, IApplicationListener>(); 

        public IReadOnlyCollection<IApplicationListener> ActivePollers
        {
            get { return activeQueuePollers.Values.OrderBy(x => x.ApplicationConfiguration.ApplicationId).ToList().AsReadOnly(); }
        }

        public void Add(IApplicationListener applicationListener)
        {
            Verify.RequireNotNull(applicationListener, "applicationListener");

            activeQueuePollers.TryAdd(applicationListener.Key, applicationListener);
        }

        public void Remove(IApplicationListener applicationListener)
        {
            Verify.RequireNotNull(applicationListener, "applicationListener");

            activeQueuePollers.TryRemove(applicationListener.Key, out applicationListener);
        }
    }
}
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RabbitOperations.Collector.Service
{
	public class ActiveApplicationListeners : IActiveApplicationListeners
	{
		private readonly ConcurrentDictionary<Guid, IApplicationListener> activeApplicationListeners = new ConcurrentDictionary<Guid, IApplicationListener>();

		public IReadOnlyCollection<IApplicationListener> ActivePollers
		{
			get { return activeApplicationListeners.Values.OrderBy(x => x.ApplicationConfiguration.ApplicationId).ToList().AsReadOnly(); }
		}

		public void Add(IApplicationListener applicationListener)
		{
			Verify.RequireNotNull(applicationListener, "applicationListener");

			activeApplicationListeners.TryAdd(applicationListener.Key, applicationListener);
		}

		public void Remove(IApplicationListener applicationListener)
		{
			Verify.RequireNotNull(applicationListener, "applicationListener");

			activeApplicationListeners.TryRemove(applicationListener.Key, out applicationListener);
		}
	}
}
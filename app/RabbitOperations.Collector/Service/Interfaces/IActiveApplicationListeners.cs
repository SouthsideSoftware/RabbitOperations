﻿using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IActiveApplicationListeners
    {
        IReadOnlyCollection<IApplicationListener> ActivePollers { get; }
        void Add(IApplicationListener applicationListener);
        void Remove(IApplicationListener applicationListener);
    }
}
using System;

namespace RabbitOperations.Collector.Poller.Interfaces
{
    public interface IPoller
    {
        void Poll();
        PollerStatus Status { get; }
    }
}
using System;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IQueuePoller : IDisposable
    {
        string QueueName { get; set; }

        void Start();
        void Stop();

        void HandleMessage(IRawMessage message);
    }
}
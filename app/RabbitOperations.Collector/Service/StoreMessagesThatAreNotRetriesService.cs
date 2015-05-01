using System;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesThatAreNotRetriesService : IStoreMessages
    {
        public void Store(IRawMessage message, IQueueSettings queueSettings)
        {
            throw new NotImplementedException();
        }
    }
}
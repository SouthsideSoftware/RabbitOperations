using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IStoreMessagesFactory
    {
        IStoreMessages MessageStorageServiceFor(IRawMessage message);
    }
}
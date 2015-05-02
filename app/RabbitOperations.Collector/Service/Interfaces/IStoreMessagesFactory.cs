using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IStoreMessagesFactory
    {
        IStoreMessages MessageStorageServiceFor(IRawMessage message);
    }
}
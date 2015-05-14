using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesFactory : IStoreMessagesFactory
    {
        public IStoreMessages MessageStorageServiceFor(IRawMessage message)
        {
            if (message.Headers.ContainsKey(Headers.Retry))
            {
                return
                    ServiceLocator.Container.Resolve<IStoreMessages>(
                        typeof (StoreMessagesThatAreRetriesService).ToString());
            }

            return
                ServiceLocator.Container.Resolve<IStoreMessages>(
                    typeof(StoreMessagesThatAreNotRetriesService).ToString());
        }
    }
}
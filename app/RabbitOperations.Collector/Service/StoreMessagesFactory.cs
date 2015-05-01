using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesFactory : IStoreMessagesFactory
    {
        public IStoreMessages MessageStorageServiceFor(IRawMessage message, IQueueSettings queueSettings)
        {
            if (message.Headers.ContainsKey(AddRetryTrackingHeadersService.RetryHeader))
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
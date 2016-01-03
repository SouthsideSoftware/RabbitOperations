using Autofac;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DependencyInjection;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesFactory : IStoreMessagesFactory
    {
        public IStoreMessages MessageStorageServiceFor(IRawMessage message)
        {
            if (message.Headers.ContainsKey(Headers.Retry))
            {
                return
                    ServiceLocator.Container.ResolveNamed<IStoreMessages>(
                        typeof (StoreMessagesThatAreRetriesService).ToString());
            }

            return
                ServiceLocator.Container.ResolveNamed<IStoreMessages>(
                    typeof(StoreMessagesThatAreNotRetriesService).ToString());
        }
    }
}
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;

namespace RabbitOperations.Collector.MessageRetry
{
    public class RetryMessagesService : IRetryMessages
    {
        public RetryMessageResult Retry(RetryMessageModel retryMessageModel)
        {
            var result = new RetryMessageResult();
            foreach (var retryId in retryMessageModel.RetryIds)
            {
                result.RetryMessageItems.Add(new RetryMessageItem
                {
                    IsRetrying = false,
                    Retryid = retryId,
                    RetryQueue = retryMessageModel.UserSuppliedRetryDestination
                });                
            }

            return result;
        }
    }
}
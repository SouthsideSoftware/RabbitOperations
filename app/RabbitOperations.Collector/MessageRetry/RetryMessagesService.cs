using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;

namespace RabbitOperations.Collector.MessageRetry
{
    public class RetryMessagesService : IRetryMessages
    {
        public RetryMessageResult Retry(long messageId)
        {
            return new RetryMessageResult
            {
                IsSuccess = false
            };
        }
    }
}
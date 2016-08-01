using RabbitOperations.Collector.Models;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface IRetryMessages
    {
        RetryMessageResult Retry(RetryMessageModel retryMessageModel);
	    RetryDestinationResult GetRetryDestinations(RetryMessageModel retryMessageModel);
    }
}

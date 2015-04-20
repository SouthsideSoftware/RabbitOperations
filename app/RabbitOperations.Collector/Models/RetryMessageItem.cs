namespace RabbitOperations.Collector.Models
{
    public class RetryMessageItem
    {
        public bool IsRetrying { get; set; }
        public long Retryid { get; set; }
    }
}
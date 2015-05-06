namespace RabbitOperations.Collector.Models
{
    public class RetryMessageItem
    {
        public bool IsRetrying { get; set; }
        public long RetryId { get; set; }
        public string RetryQueue { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
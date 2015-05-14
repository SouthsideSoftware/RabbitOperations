namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IQueueSettings
    {
        string ApplicationId { get; }
        string ApplicationName { get; }
        string QueueName { get; }

        int MaxMessagesPerRun { get; }
        int PollingTimeoutMilliseconds { get; }
        string RabbitConnectionString { get; }
        int HeartbeatIntervalSeconds { get; }
        int DocumentExpirationInHours { get; }
        bool IsErrorQueue { get; }
        string LogInfo { get; }
        ushort Prefetch { get; set; }
    }
}
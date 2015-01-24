namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IQueueSettings
    {
        string EnvironmentId { get; }
        string EnvironmentName { get; }
        string QueueName { get; }

        int MaxMessagesPerRun { get; }
        int PollingTimeout { get; }
        string RabbitConnectionString { get; }
        int HeartbeatIntervalSeconds { get; }
    }
}
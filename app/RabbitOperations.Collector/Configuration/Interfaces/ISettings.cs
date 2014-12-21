namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {
        string AuditQueue { get; }
        string ErrorQueue { get; }
    }
}

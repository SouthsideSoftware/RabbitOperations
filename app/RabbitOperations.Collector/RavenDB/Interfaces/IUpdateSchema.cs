namespace RabbitOperations.Collector.RavenDB.Interfaces
{
    public interface IUpdateSchema
    {
        int SchemaVersion { get; }
        void UpdateSchema();
    }
}
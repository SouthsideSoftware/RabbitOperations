namespace RabbitOperations.Collector.RavenDB.Interfaces
{
    public interface IUpdateSchemaVersion
    {
        int SchemaVersion { get; }
        void UpdateSchema();
    }
}
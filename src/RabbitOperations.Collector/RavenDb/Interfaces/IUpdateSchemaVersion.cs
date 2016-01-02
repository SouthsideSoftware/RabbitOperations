namespace RabbitOperations.Collector.RavenDb.Interfaces
{
    public interface IUpdateSchemaVersion
    {
        int SchemaVersion { get; }
        void UpdateSchema();
    }
}
namespace RabbitOperations.Collector.RavenDb.Interfaces
{
    public interface IRavenTenantInitializer
    {
        void InitializeTenant(string tenantName);
    }
}
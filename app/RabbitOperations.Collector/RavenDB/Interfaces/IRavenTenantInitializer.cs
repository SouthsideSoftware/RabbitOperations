namespace RabbitOperations.Collector.RavenDB.Interfaces
{
    public interface IRavenTenantInitializer
    {
        void InitializeTenant(string tenantName);
    }
}
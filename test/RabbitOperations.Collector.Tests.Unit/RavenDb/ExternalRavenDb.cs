using RabbitOperations.Collector.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.RavenDb
{
    public class ExternalRavenDb : RavenDbTest
    {
        public ExternalRavenDb() : base(ravenInMemory: false)
        {
        }
    }
}
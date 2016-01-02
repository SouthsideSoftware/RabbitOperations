using RabbitOperations.Collector.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.RavenDb
{
    public class RavenDbInMemory : RavenDbTest
    {
        public RavenDbInMemory() : base(ravenInMemory: true)
        {
        }
    }
}
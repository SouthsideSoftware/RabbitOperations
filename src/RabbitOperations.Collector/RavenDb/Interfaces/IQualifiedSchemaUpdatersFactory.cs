using System.Collections.Generic;

namespace RabbitOperations.Collector.RavenDb.Interfaces
{
    public interface IQualifiedSchemaUpdatersFactory
    {
        IReadOnlyList<IUpdateSchemaVersion> Get();
    }
}
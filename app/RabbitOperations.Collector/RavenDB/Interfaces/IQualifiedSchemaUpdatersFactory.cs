using System.Collections.Generic;

namespace RabbitOperations.Collector.RavenDB.Interfaces
{
    public interface IQualifiedSchemaUpdatersFactory
    {
        IReadOnlyList<IUpdateSchemaVersion> Get();
    }
}
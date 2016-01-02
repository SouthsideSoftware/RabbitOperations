using System.Collections.Generic;
using System.Linq;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb
{
    public class QualifiedSchemaUpdatersFactory : IQualifiedSchemaUpdatersFactory
    {
        private readonly IEnumerable<IUpdateSchemaVersion> schemaUpdaters;
        private readonly ISettings settings;

        public QualifiedSchemaUpdatersFactory(IEnumerable<IUpdateSchemaVersion> schemaUpdaters, ISettings settings)
        {
            Verify.RequireNotNull(schemaUpdaters, "schemaUpdaters");
            Verify.RequireNotNull(settings, "settings");

            this.schemaUpdaters = schemaUpdaters;
            this.settings = settings;
        }

        public IReadOnlyList<IUpdateSchemaVersion> Get()
        {
            return
                schemaUpdaters.Where(x => x.SchemaVersion > settings.DatabaseSchemaVersion)
                    .OrderBy(x => x.SchemaVersion)
                    .ToList()
                    .AsReadOnly();
        }
    }
}
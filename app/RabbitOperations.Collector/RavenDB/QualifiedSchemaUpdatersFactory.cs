using System.Collections.Generic;
using System.Linq;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public class QualifiedSchemaUpdatersFactory : IQualifiedSchemaUpdatersFactory
    {
        private readonly IList<IUpdateSchemaVersion> schemaUpdaters;
        private readonly ISettings settings;

        public QualifiedSchemaUpdatersFactory(IList<IUpdateSchemaVersion> schemaUpdaters, ISettings settings)
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
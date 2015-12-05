using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.SchemaUpdates
{
    public class ToVersion0014 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0014(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 14; }
        }

        public void UpdateSchema()
        {
            Log.Logger.Information("Updating MessageDocumentSearch index");
            store.ExecuteIndexCreationOnDefaultTenant(new MessageDocument_Search());
        }
    }
}
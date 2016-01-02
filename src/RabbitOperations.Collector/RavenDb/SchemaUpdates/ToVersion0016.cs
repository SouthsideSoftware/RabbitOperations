using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.SchemaUpdates
{
    public class ToVersion0016 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0016(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 16; }
        }

        public void UpdateSchema()
        {
            Log.Logger.Information("Updating MessageDocument structure");
            store.ExecuteUpdateByIndexOnDefaultTenant("MessageDocument/Search",
                new IndexQuery(),
                new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Rename,
                        Name = "EnvironmentId",
                        Value = new RavenJValue("ApplicationId")
                    }
                });
        }
    }
}
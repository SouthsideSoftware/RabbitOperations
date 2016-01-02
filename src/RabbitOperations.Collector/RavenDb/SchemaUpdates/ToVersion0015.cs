using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Client;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.SchemaUpdates
{
    public class ToVersion0015 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0015(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 15; }
        }

        public void UpdateSchema()
        {
            //Update config document by saving
            Log.Logger.Information("Updating structure of configuration document");
            settings.Save();
        }
    }
}
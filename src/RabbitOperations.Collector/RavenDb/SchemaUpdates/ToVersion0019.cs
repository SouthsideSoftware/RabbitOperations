﻿using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Indexes;
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Client;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.SchemaUpdates
{
    public class ToVersion0019 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0019(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 19; }
        }

        public void UpdateSchema()
        {
            Log.Logger.Information("Updating MessageDocumentSearch index");
            store.ExecuteIndexCreationOnDefaultTenant(new MessageDocument_Search());
        }
    }
}
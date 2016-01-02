﻿using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Client;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.SchemaUpdates
{
    public class ToVersion0018 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0018(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 18; }
        }

        public void UpdateSchema()
        {
            //Update config document by saving
            Log.Logger.Information("Updating structure of configuration document");
            settings.Save();
        }
    }
}
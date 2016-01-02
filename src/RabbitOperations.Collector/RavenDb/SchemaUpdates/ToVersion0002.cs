using System;
using Microsoft.Extensions.OptionsModel;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDb.Indexes;
using RabbitOperations.Collector.RavenDb.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.SchemaUpdates
{
    public class ToVersion0002 : IUpdateSchemaVersion
    {
        private readonly RavenDbSettings ravenDbSettings;
        private readonly ISettings settings;
        private readonly IDocumentStore store;

        public ToVersion0002(IOptions<RavenDbSettings> ravenDbSettings, ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "ravenDbSettings");
            Verify.RequireNotNull(store, "store");
            Verify.RequireNotNull(settings, "settings");

            this.ravenDbSettings = ravenDbSettings.Value;
            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 2; }
        }

        public void UpdateSchema()
        {
            Log.Logger.Information("Creating MessageDocumentSearch index");
            store.ExecuteIndexCreationOnDefaultTenant(new MessageDocument_Search());
            //Update config document by saving
            Log.Logger.Information("Updating structure of configuration document");
            settings.Save();
            Log.Logger.Information("Patching existing message documents to expire in 7 days");
            SetExpiration();
        }

        private void SetExpiration()
        {
            var expiry = DateTime.UtcNow.AddHours(7*24);
            store.DatabaseCommands.ForDatabase(ravenDbSettings.DefaultTenant)
                .UpdateByIndex("Raven/DocumentsByEntityName",
                    new IndexQuery {Query = "Tag:MessageDocuments"},
                    new[]
                    {
                        new PatchRequest()
                        {
                            Type = PatchCommandType.Modify,
                            Name = "@metadata",
                            Value = new RavenJObject(),
                            Nested = new[]
                            {
                                new PatchRequest()


                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Raven-Expiration-Date",
                                    Value = new RavenJValue(expiry),
                                }
                            }
                        }
                    });
        }
    }
}

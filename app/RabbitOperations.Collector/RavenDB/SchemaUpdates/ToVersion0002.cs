using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Interfaces;
using RabbitOperations.Collector.Service;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Json.Linq;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.SchemaUpdates
{
    public class ToVersion0002 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        private readonly IDocumentStore store;
        public Logger logger = LogManager.GetCurrentClassLogger();

        public ToVersion0002(ISettings settings, IDocumentStore store)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(store, "store");

            this.settings = settings;
            this.store = store;
        }

        public int SchemaVersion
        {
            get { return 2; }
        }

        public void UpdateSchema()
        {
            logger.Info("Creating MessageDocumentSearch index");
            store.ExecuteIndexCreationOnDefaultTenant(new MessageDocument_Search());
            //Update config document by saving
            logger.Info("Updating structure of configuration document");
            settings.Save();
            logger.Info("Patching existing message documents to expire in 7 days");
            SetExpiration();
        }

        private void SetExpiration()
        {
            var expiry = DateTime.UtcNow.AddHours(7*24);
            store.DatabaseCommands.ForDatabase(settings.DefaultRavenDBTenant)
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

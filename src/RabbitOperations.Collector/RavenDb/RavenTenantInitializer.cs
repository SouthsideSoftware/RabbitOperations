using System.Collections.Generic;
using System.Linq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public class RavenTenantInitializer : IRavenTenantInitializer
    {
        private readonly IDocumentStore docStore;

        public RavenTenantInitializer(IDocumentStore docStore)
        {
            Verify.RequireNotNull(docStore, "docStore");

            this.docStore = docStore;
        }

        public void InitializeTenant(string tenantName)
        {
            if (
                docStore.DatabaseCommands.GlobalAdmin.GetDatabaseNames(1000)
                    .All(dbName => dbName != Settings.StaticDefaultRavenDBTenant))
            {
                var databaseDocument = new DatabaseDocument
                {
                    Id = string.Format("Raven/Databases/{0}", Settings.StaticDefaultRavenDBTenant),
                    Settings = new Dictionary<string, string>
                    {
                        {"Raven/ActiveBundles", "DocumentExpiration"},
                        {"Raven/DataDir", string.Format("~/Databases/{0}", Settings.StaticDefaultRavenDBTenant)}
                    }
                };
                docStore.DatabaseCommands.GlobalAdmin.CreateDatabase(databaseDocument);
            }
        }
    }
}
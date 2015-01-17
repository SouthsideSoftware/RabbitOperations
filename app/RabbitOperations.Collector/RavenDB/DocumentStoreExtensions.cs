using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.Configuration;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public static class DocumentStoreExtensions
    {
        public static IDocumentSession OpenSessionForDefaultTenant(this IDocumentStore documentStore)
        {
            Verify.RequireNotNull(documentStore, "documentStore");

            return documentStore.OpenSession(Settings.StaticDefaultRavenDBTenant);
        }
    }
}

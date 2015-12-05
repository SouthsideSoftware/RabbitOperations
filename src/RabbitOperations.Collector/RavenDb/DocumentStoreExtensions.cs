using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using RabbitOperations.Collector.Configuration;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using SouthsideUtility.Core;
using SouthsideUtility.Core.DependencyInjection;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public static class DocumentStoreExtensions
    {
        public static IDocumentSession OpenSessionForDefaultTenant(this IDocumentStore documentStore)
        {
            Verify.RequireNotNull(documentStore, "documentStore");


            return documentStore.OpenSession(RavenDbSettingsHelper.Instance.DefaultTenant);
        }

        public static void ExecuteIndexCreationOnDefaultTenant(this IDocumentStore documentStore, AbstractIndexCreationTask indexCreationTask)
        {
            Verify.RequireNotNull(documentStore, "documentStore");

            indexCreationTask.Execute(documentStore.DatabaseCommands.ForDatabase(RavenDbSettingsHelper.Instance.DefaultTenant),
                documentStore.Conventions);
        }

        public static void ExecuteUpdateByIndexOnDefaultTenant(this IDocumentStore documentStore, string indexName,
            IndexQuery indexQuery, PatchRequest[] patchRequests)
        {
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireStringNotNullOrWhitespace(indexName, "indexName");
            Verify.RequireNotNull(indexQuery, "indexQuery");
            Verify.RequireNotNull(patchRequests, "patchRequests");

            documentStore.DatabaseCommands.ForDatabase(RavenDbSettingsHelper.Instance.DefaultTenant)
                .UpdateByIndex(indexName, indexQuery, patchRequests);
        }
    }
}

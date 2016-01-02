using RabbitOperations.Collector.Configuration;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb
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

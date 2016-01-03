using System;
using Polly;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.RavenDb;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesThatAreNotRetriesService : IStoreMessages
    {
        private readonly IDocumentStore documentStore;
        private readonly IHeaderParser headerParser;

        public StoreMessagesThatAreNotRetriesService(IHeaderParser headerParser, IDocumentStore documentStore)
        {
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");

            this.headerParser = headerParser;
            this.documentStore = documentStore;
        }

        public long Store(IRawMessage message, IQueueSettings queueSettings)
        {
            var document = new MessageDocument
            {
                ApplicationId = queueSettings.ApplicationId
            };
            headerParser.AddHeaderInformation(message, document);
            document.Body = message.Body;
            var expiry =
                DateTime.UtcNow.AddHours(document.IsError
                    ? queueSettings.ErrorDocumentExpirationInHours
                    : queueSettings.DocumentExpirationInHours);

            //deal with rare transients that happen under load
            var policy =
                Policy.Handle<Exception>()
                    .WaitAndRetry(new[] {TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(10)},
                        (exception, retryDelay, context) =>
                        {
                            Log.Error(exception,
                                $"Retrying storage of message document with delay {retryDelay} after exception");
                        });
            policy.Execute(() => SaveDocument(queueSettings, document, expiry));

            return document.Id;
        }

        private void SaveDocument(IQueueSettings queueSettings, MessageDocument document, DateTime expiry)
        {
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                session.Store(document);
                session.Advanced.GetMetadataFor(document)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                session.SaveChanges();
                Log.Verbose($"Saved document for message with id {document.Id} from {queueSettings.LogInfo}");
            }
        }
    }
}
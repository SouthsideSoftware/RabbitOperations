using System;
using System.Reflection;
using NLog;
using Polly;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using Raven.Json.Linq;
using SouthsideUtility.Core.DesignByContract;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesThatAreNotRetriesService : IStoreMessages
    {
        private readonly IHeaderParser headerParser;
        private readonly IDocumentStore documentStore;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StoreMessagesThatAreNotRetriesService(IHeaderParser headerParser, IDocumentStore documentStore)
        {
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");

            this.headerParser = headerParser;
            this.documentStore = documentStore;
        }

        public long Store(IRawMessage message, IApplicationConfiguration applicationConfiguration)
        {
            var document = new MessageDocument
            {
                ApplicationId = applicationConfiguration.ApplicationId
            };
            headerParser.AddHeaderInformation(message, document);
            document.Body = message.Body;
            var expiry = DateTime.UtcNow.AddHours(document.IsError ? applicationConfiguration.ErrorDocumentExpirationInHours : applicationConfiguration.DocumentExpirationInHours);

            //deal with rare transients that happen under load
            var policy = Policy.Handle<Exception>().WaitAndRetry(new TimeSpan[] {TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(10)}, (exception, retryDelay, context) =>
            {
                logger.Error(exception, $"Retrying storage of message document with delay {retryDelay} after exception");
            });
            policy.Execute(() => SaveDocument(applicationConfiguration, document, expiry));

            return document.Id;
        }

        private void SaveDocument(IApplicationConfiguration applicationConfiguration, MessageDocument document, DateTime expiry)
        {
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                session.Store(document);
                session.Advanced.GetMetadataFor(document)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                session.SaveChanges();
                logger.Trace($"Saved document for message with id {document.Id} from {applicationConfiguration.ApplicationLogInfo}");
            }
        }
    }
}
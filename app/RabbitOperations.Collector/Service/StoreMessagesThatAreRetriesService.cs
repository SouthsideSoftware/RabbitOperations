using System;
using System.Collections.Generic;
using NLog;
using Polly;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using Raven.Json.Linq;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class StoreMessagesThatAreRetriesService : IStoreMessages
    {
        private readonly IHeaderParser headerParser;
        private readonly IDocumentStore documentStore;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StoreMessagesThatAreRetriesService(IHeaderParser headerParser, IDocumentStore documentStore)
        {
            Verify.RequireNotNull(headerParser, "headerParser");
            Verify.RequireNotNull(documentStore, "documentStore");

            this.headerParser = headerParser;
            this.documentStore = documentStore;
        }

        public long Store(IRawMessage message, IQueueSettings queueSettings)
        {
            var retry = new MessageDocument
            {
                ApplicationId = queueSettings.ApplicationId
            };
            headerParser.AddHeaderInformation(message, retry);
            retry.Body = message.Body;
            var expiry = DateTime.UtcNow.AddHours(queueSettings.DocumentExpirationInHours);

            long originalId = -1;
            long.TryParse(message.Headers[Headers.Retry], out originalId);
            long documentIdToReturn = -1;

            //deal with rare transients that happen under load
            var policy = Policy.Handle<Exception>().WaitAndRetry(new TimeSpan[] { TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(10) }, (exception, retryDelay, context) =>
            {
                logger.ErrorException($"Retrying storage of message document with delay {retryDelay} after exception", exception);
            });
            policy.Execute(() =>
            {
                documentIdToReturn = SaveDocument(queueSettings, originalId, retry, expiry);
            });

            return documentIdToReturn;
        }

        private long SaveDocument(IQueueSettings queueSettings, long originalId, MessageDocument retry, DateTime expiry)
        {
            long documentIdToReturn = -1;
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                MessageDocument originalMessage = null;
                if (originalId > 0)
                {
                    originalMessage = session.Load<MessageDocument>(originalId);
                }
                if (originalMessage != null)
                {
                    if (originalMessage.Retries == null)
                    {
                        originalMessage.Retries = new List<MessageDocument>();
                    }
                    originalMessage.Retries.Add(retry);
                    if (retry.IsError)
                    {
                        originalMessage.AdditionalErrorStatus = AdditionalErrorStatus.Unresolved;
                    }
                    else
                    {
                        originalMessage.AdditionalErrorStatus = AdditionalErrorStatus.Resolved;
                    }
                    session.Advanced.GetMetadataFor(originalMessage)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                    documentIdToReturn = originalMessage.Id;
                    logger.Trace(
                        "Added retry information with additional error status {0} to existing document with id {1} from {2}",
                        retry.AdditionalErrorStatus, originalMessage.Id, queueSettings.LogInfo);
                }
                else
                {
                    session.Store(retry);
                    session.Advanced.GetMetadataFor(retry)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                    documentIdToReturn = retry.Id;
                    logger.Trace("Saved new document for retry message with id {0} from {1}", retry.Id, queueSettings.LogInfo);
                }
                session.SaveChanges();
            }
            return documentIdToReturn;
        }
    }
}
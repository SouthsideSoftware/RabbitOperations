﻿using System;
using NLog;
using RabbitOperations.Collector.MessageParser.Interfaces;
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
            var document = new MessageDocument
            {
                ApplicationId = queueSettings.ApplicationId
            };
            headerParser.AddHeaderInformation(message, document);
            document.Body = message.Body;
            var expiry = DateTime.UtcNow.AddHours(queueSettings.DocumentExpirationInHours);

            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                session.Store(document);
                session.Advanced.GetMetadataFor(document)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                session.SaveChanges();
                logger.Trace("Saved document for message with id {0} from {1}", document.Id, queueSettings.LogInfo);
            }

            return document.Id;
        }
    }
}
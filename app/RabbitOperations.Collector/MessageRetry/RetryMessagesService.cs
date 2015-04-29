using System.Runtime.Remoting.Messaging;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageRetry
{
    public class RetryMessagesService : IRetryMessages
    {
        private readonly ICreateRetryMessagesFromOriginal createRetryMessagesFromOriginalService;
        private readonly IDetermineRetryDestination determineRetryDestinationService;
        private readonly IDocumentStore documentStore;

        public RetryMessagesService(ICreateRetryMessagesFromOriginal createRetryMessagesFromOriginalService,
            IDetermineRetryDestination determineRetryDestinationService, IDocumentStore documentStore)
        {
            Verify.RequireNotNull(createRetryMessagesFromOriginalService, "createRetryMessagesFromOriginalService");
            Verify.RequireNotNull(determineRetryDestinationService, "determineRetryDestinationService");
            Verify.RequireNotNull(documentStore, "documentStore");

            this.createRetryMessagesFromOriginalService = createRetryMessagesFromOriginalService;
            this.determineRetryDestinationService = determineRetryDestinationService;
            this.documentStore = documentStore;
        }

        public RetryMessageResult Retry(RetryMessageModel retryMessageModel)
        {
            var result = new RetryMessageResult();
            foreach (var retryId in retryMessageModel.RetryIds)
            {
                MessageDocument originalMessage = GetOriginalMessageIfExists(retryId, result);
                if (originalMessage == null) continue;
     
                var rawMessage = new RawMessage(originalMessage);
                createRetryMessagesFromOriginalService.PrepareMessageForRetry(rawMessage);
                var destination = determineRetryDestinationService.GetRetryDestination(rawMessage,
                    retryMessageModel.UserSuppliedRetryDestination);
                //setup the RetryId header
                //send it
                //add information about the retry to the collection
                result.RetryMessageItems.Add(new RetryMessageItem
                {
                    IsRetrying = false,
                    Retryid = retryId,
                    RetryQueue = retryMessageModel.UserSuppliedRetryDestination
                });                
            }

            return result;
        }

        private MessageDocument GetOriginalMessageIfExists(long retryId, RetryMessageResult result)
        {
            MessageDocument originalMessage;
            using (var session = documentStore.OpenSessionForDefaultTenant())
            {
                originalMessage = session.Load<MessageDocument>(retryId);
            }
            if (originalMessage == null)
            {
                result.RetryMessageItems.Add(new RetryMessageItem
                {
                    IsRetrying = false,
                    Retryid = retryId,
                    RetryQueue = null,
                    AdditionalInfo = "Original message does not exist in message store"
                });
                return null;
            }
            return originalMessage;
        }
    }
}
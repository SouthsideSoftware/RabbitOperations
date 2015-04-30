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
        private readonly IAddRetryTrackingHeaders addRetryTrackingHeadersService;
        private readonly IDocumentStore documentStore;

        public RetryMessagesService(ICreateRetryMessagesFromOriginal createRetryMessagesFromOriginalService,
            IDetermineRetryDestination determineRetryDestinationService, IAddRetryTrackingHeaders addRetryTrackingHeadersService, IDocumentStore documentStore)
        {
            Verify.RequireNotNull(createRetryMessagesFromOriginalService, "createRetryMessagesFromOriginalService");
            Verify.RequireNotNull(determineRetryDestinationService, "determineRetryDestinationService");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(addRetryTrackingHeadersService, "addRetryTrackingHeadersService");

            this.createRetryMessagesFromOriginalService = createRetryMessagesFromOriginalService;
            this.determineRetryDestinationService = determineRetryDestinationService;
            this.addRetryTrackingHeadersService = addRetryTrackingHeadersService;
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
                addRetryTrackingHeadersService.AddTrackingHeaders(rawMessage, retryId);

                //send it
                //add information about the retry to the collection
                result.RetryMessageItems.Add(new RetryMessageItem
                {
                    IsRetrying = true,
                    Retryid = retryId,
                    RetryQueue = destination
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
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
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
        private readonly IAddRetryTrackingHeaders addRetryTrackingHeadersService;
        private readonly ICreateBasicProperties createBasicPropertiesService;
        private readonly ICreateRetryMessagesFromOriginal createRetryMessagesFromOriginalService;
        private readonly IDetermineRetryDestination determineRetryDestinationService;
        private readonly IDocumentStore documentStore;
        private readonly ISendMessages sendMessagesService;

        public RetryMessagesService(ICreateRetryMessagesFromOriginal createRetryMessagesFromOriginalService,
            IDetermineRetryDestination determineRetryDestinationService,
            IAddRetryTrackingHeaders addRetryTrackingHeadersService, IDocumentStore documentStore,
            ISendMessages sendMessagesService, ICreateBasicProperties createBasicPropertiesService)
        {
            Verify.RequireNotNull(createRetryMessagesFromOriginalService, "createRetryMessagesFromOriginalService");
            Verify.RequireNotNull(determineRetryDestinationService, "determineRetryDestinationService");
            Verify.RequireNotNull(documentStore, "documentStore");
            Verify.RequireNotNull(addRetryTrackingHeadersService, "addRetryTrackingHeadersService");
            Verify.RequireNotNull(createBasicPropertiesService, "createBasicPropertiesService");

            this.createRetryMessagesFromOriginalService = createRetryMessagesFromOriginalService;
            this.determineRetryDestinationService = determineRetryDestinationService;
            this.addRetryTrackingHeadersService = addRetryTrackingHeadersService;
            this.documentStore = documentStore;
            this.sendMessagesService = sendMessagesService;
            this.createBasicPropertiesService = createBasicPropertiesService;
        }

        public RetryMessageResult Retry(RetryMessageModel retryMessageModel)
        {
            var result = new RetryMessageResult();
            foreach (var retryId in retryMessageModel.RetryIds)
            {
                var originalMessage = GetOriginalMessageIfExists(retryId, result);
                if (originalMessage == null) continue;

                var rawMessage = new RawMessage(originalMessage);
                var destination = determineRetryDestinationService.GetRetryDestination(rawMessage,
                    retryMessageModel.UserSuppliedRetryDestination);
                createRetryMessagesFromOriginalService.PrepareMessageForRetry(rawMessage);
                addRetryTrackingHeadersService.AddTrackingHeaders(rawMessage, retryId);
                IBasicProperties basicProperties = createBasicPropertiesService.Create(rawMessage);
                var errorMessage = sendMessagesService.Send(rawMessage, destination, originalMessage.ApplicationId,
                    basicProperties);
                if (errorMessage == null)
                {
                    result.RetryMessageItems.Add(new RetryMessageItem
                    {
                        IsRetrying = true,
                        Retryid = retryId,
                        RetryQueue = destination,
                        AdditionalInfo = null
                    });
                    using (var session = documentStore.OpenSessionForDefaultTenant())
                    {
                        originalMessage.AdditionalErrorStatus = AdditionalErrorStatus.RetryPending;
                        session.Store(originalMessage);
                        session.SaveChanges();
                    }
                }
                else
                {
                    result.RetryMessageItems.Add(new RetryMessageItem
                    {
                        IsRetrying = false,
                        Retryid = retryId,
                        RetryQueue = destination,
                        AdditionalInfo = errorMessage
                    });
                }
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
using System.Runtime.Remoting.Messaging;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
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
                //get message
                //construct new rawmessage
                //run through the createRetryMessage
                //get the destination
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
    }
}
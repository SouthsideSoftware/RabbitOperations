using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageBusTechnologies.NServiceBus
{
    public class DetermineRetryDestinationService : IDetermineRetryDestination
    {
        public string GetRetryDestination(IRawMessage rawMessage, string userSuppliedRetryDestination)
        {
            Verify.RequireNotNull(rawMessage, "rawMessage");

            if (!string.IsNullOrWhiteSpace(userSuppliedRetryDestination))
            {
                return userSuppliedRetryDestination;
            }

	        if (rawMessage.Headers.ContainsKey(Headers.FailedQ))
	        {
		        var failedQ = rawMessage.Headers[Headers.FailedQ];
		        return failedQ.Split('@')[0];
	        }
	        if (rawMessage.Headers.ContainsKey(Headers.ProcessingEndpoint))
	        {
		        return rawMessage.Headers[Headers.ProcessingEndpoint];
	        }
            return null;
        }
    }
}
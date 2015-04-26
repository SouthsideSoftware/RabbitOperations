using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.NServiceBus
{
    public class DetermineRetryDestination : IDetermineRetryDestination
    {
        public string GetRetryDestination(IRawMessage rawMessage)
        {
            return null;
        }
    }
}
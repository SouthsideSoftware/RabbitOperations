using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface IDetermineRetryDestination
    {
        string GetRetryDestination(IRawMessage rawMessage, string userSuppliedRetryDestination);
    }
}
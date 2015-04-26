using RabbitOperations.Collector.MessageParser.Interfaces;

namespace RabbitOperations.Collector.MessageRetry.Interfaces
{
    public interface IDetermineRetryDestination
    {
        string GetRetryDestination(IRawMessage rawMessage);
    }
}
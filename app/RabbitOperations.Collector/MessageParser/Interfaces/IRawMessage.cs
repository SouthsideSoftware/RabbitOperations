using System.Collections.Generic;

namespace RabbitOperations.Collector.MessageParser.Interfaces
{
    public interface IRawMessage
    {
        IDictionary<string, string> Headers { get; } 
        string Body { get; }
    }
}
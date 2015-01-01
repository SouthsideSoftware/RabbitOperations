using System.Collections.Generic;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IRawMessage
    {
        IDictionary<string, string> Headers { get; } 
        string Body { get; }
    }
}
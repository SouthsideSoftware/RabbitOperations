using System.Collections.Generic;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IRawMessage
    {
        Dictionary<string, string> Headers { get; } 
        string Data { get; }
    }
}
using System;
using System.Collections.Generic;

namespace RabbitOperations.Domain
{
    public interface IRawMessage
    {
        IDictionary<string, string> Headers { get; } 
        string Body { get; }
        Tuple<byte[], Dictionary<string, object>> GetEelementsForRabbitPublish();
    }
}
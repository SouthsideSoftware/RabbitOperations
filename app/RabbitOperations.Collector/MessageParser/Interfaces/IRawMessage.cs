using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace RabbitOperations.Collector.MessageParser.Interfaces
{
    public interface IRawMessage
    {
        IDictionary<string, string> Headers { get; } 
        string Body { get; }
        Tuple<byte[], Dictionary<string, object>> GetEelementsForRabbitPublish();
    }
}
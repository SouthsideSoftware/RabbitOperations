using System;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.MessageParser
{
    public class BodyParser : IBodyParser
    {
        public void ParseBody(IRawMessage message, MessageDocument messageDocument)
        {
            messageDocument.Body = message.Body;
        }
    }
}
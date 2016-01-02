using System;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.MessageParser
{
    public class BodyParser : IBodyParser
    {
        private readonly ISettings settings;

        public BodyParser(ISettings settings)
        {
            Verify.RequireNotNull(settings, "settings");

            this.settings = settings;
        }

        public void ParseBody(IRawMessage message, MessageDocument messageDocument)
        {
            messageDocument.Body = message.Body;
        }
    }
}
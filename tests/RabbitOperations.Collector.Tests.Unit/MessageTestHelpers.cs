using System.IO;
using Newtonsoft.Json;
using RabbitOperations.Collector.MessageParser;

namespace RabbitOperations.Collector.Tests.Unit
{
    public static class MessageTestHelpers
    {
        public static RawMessage GetErrorMessage()
        {
            return GetMessageFromFile("Error.json");
        }

        public static RawMessage GetAuditMessage()
        {
            return GetMessageFromFile("Audit.json");
        }

        public static RawMessage GetMessageFromFile(string fileName)
        {
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", fileName)))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            return rawMessage;
        }
    }
}
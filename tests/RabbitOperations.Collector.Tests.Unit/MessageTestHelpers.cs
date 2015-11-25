using System.IO;
using Newtonsoft.Json;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;
using SouthsideUtility.Testing;

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
            using (var reader = new StreamReader(TestHelper.FullPath(Path.Combine("../../TestData", fileName))))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            return rawMessage;
        }
    }
}
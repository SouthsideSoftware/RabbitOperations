using System;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using Newtonsoft.Json;
using RabbitOperations.Domain;

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
            using (var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", $"{fileName}.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            return rawMessage;
        }
    }
}
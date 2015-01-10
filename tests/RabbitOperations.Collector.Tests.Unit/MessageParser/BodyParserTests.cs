using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class BodyParserTests
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

        [Test]
        public void BodyParserCapturesRawBodyFromAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var bodyParser = new BodyParser();
            var doc = new MessageDocument();

            //act
            bodyParser.ParseBody(rawMessage, doc);

            //assert
            doc.Body.Should().Be("{\"CorrelationGuid\":\"f349702d-1be6-4c65-8f74-de8457ed4ccf\"}");
        }

        [Test]
        public void BodyParserCapturesRawBodyFromError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var bodyParser = new BodyParser();
            var doc = new MessageDocument();

            //act
            bodyParser.ParseBody(rawMessage, doc);

            //assert
            doc.Body.Should().Be("{\"Ids\":[\"afecc831-34d4-47ca-b43b-56eb90d4e3b6\"]}");
        }
    }
}
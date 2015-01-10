using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class KeyExtractorTests
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

        [Test]
        public void KeyParserExtractsConfiguredKeysFromAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            const string keyJsonPath = "CorrelationGuid";
            var mockSettings = new Mock<ISettings>();
            var messageType = "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            mockSettings.Setup(
                x =>
                    x.MessageTypeHandlingFor(
                        messageType))
                .Returns(new MessageTypeHandling
                {
                    KeyPaths = new List<JsonPath>
                    {
                        new JsonPath(keyJsonPath)
                    }
                });
            var mocker = new AutoMocker();
            mocker.Use(mockSettings);
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var keyExtraxtor = mocker.CreateInstance<KeyExtractor>() ;
            var doc = new MessageDocument();

            //act
            var keys = keyExtraxtor.GetBusinessKeys(rawMessage.Body, messageType);

            //assert
            keys.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {keyJsonPath, "f349702d-1be6-4c65-8f74-de8457ed4ccf"}
            });
        }

        [Test]
        public void KeyParserExtractsConfiguredKeysFromError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            const string keyJsonPath = "Ids";
            var mockSettings = new Mock<ISettings>();
            var messageType = "Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null";
            mockSettings.Setup(
                x =>
                    x.MessageTypeHandlingFor(
                        messageType))
                .Returns(new MessageTypeHandling
                {
                    KeyPaths = new List<JsonPath>
                    {
                        new JsonPath(keyJsonPath)
                    }
                });
            var mocker = new AutoMocker();
            mocker.Use(mockSettings);
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var keyExtraxtor = mocker.CreateInstance<KeyExtractor>();
            var doc = new MessageDocument();

            //act
            var keys = keyExtraxtor.GetBusinessKeys(rawMessage.Body, messageType);

            //assert
            keys.ShouldBeEquivalentTo(new Dictionary<string, string>
            {
                {keyJsonPath, "afecc831-34d4-47ca-b43b-56eb90d4e3b6"}
            });
        }
    }
}
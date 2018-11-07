using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Testing;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class KeyExtractorTests
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffffffZ";
        private const string ApplicationJsonContentType = "application/json";

        [Test]
        public void KeyParserExtractsConfiguredKeysFromAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(TestHelper.FullPath(Path.Combine("../../TestData", "Audit.json"))))
            {
                data = reader.ReadToEnd();
            }
            const string keyJsonPath = "CorrelationGuid";
            var messageType =
                "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var mockSettings = Mock.Of<ISettings>(x => x.MessageTypeHandlingFor(messageType) == new MessageTypeHandling
            {
                KeyPaths = new List<JsonPath>
                {
                    new JsonPath(keyJsonPath)
                }
            });
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => mockSettings);
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var keyExtractor = fixture.Create<KeyExtractor>();
            var doc = new MessageDocument();

            //act
            var keys = keyExtractor.GetBusinessKeys(rawMessage.Body, messageType);

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
            using (var reader = new StreamReader(TestHelper.FullPath(Path.Combine("../../TestData", "Error.json"))))
            {
                data = reader.ReadToEnd();
            }
            const string keyJsonPath = "Ids";
            var messageType =
                "Autobahn.Configurations.Contracts.Commands.ValidateConfigurations, Autobahn.Configurations.Contracts, Version=1.1.12.0, Culture=neutral, PublicKeyToken=null";
            var mockSettings = Mock.Of<ISettings>(x => x.MessageTypeHandlingFor(messageType) == new MessageTypeHandling
            {
                KeyPaths = new List<JsonPath>
                {
                    new JsonPath(keyJsonPath)
                }
            });
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => mockSettings);
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            var keyExtraxtor = fixture.Create<KeyExtractor>();
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

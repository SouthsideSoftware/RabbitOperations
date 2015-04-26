using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageRetry.NServiceBus;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry.NServiceBus
{
    public class CreateRetryMessageFromOriginalTests
    {
        [Test]
        public void ShouldRemoveDiagnosticHeaders()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginal();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.FirstOrDefault(x => x.Key.StartsWith("$.diagnostics")).Should().BeNull();
        }

        [Test]
        public void ShouldRemoveErrorHeaders()
        {
            var errorHeaders = new List<string> {"NServiceBus.FLRetries", "NServiceBus.Retries", "NServiceBus.FailedQ"};
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginal();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Select(x => x.Key).Intersect(errorHeaders).Should().BeEmpty();
        }

        [Test]
        public void ShouldRemoveProcessingHeaders()
        {
            var processingHeaders = new List<string> { "NServiceBus.Version", "NServiceBus.TimeSent", "NServiceBus.EnclosedMessageTypes", "NServiceBus.ProcessingStarted", "NServiceBus.ProcessingEnded", "NServiceBus.OriginatingAddress", "NServiceBus.ProcessingEndpoint", "NServiceBus.ProcessingMachine" };
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginal();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Select(x => x.Key).Intersect(processingHeaders).Should().BeEmpty();
        }
    }
}

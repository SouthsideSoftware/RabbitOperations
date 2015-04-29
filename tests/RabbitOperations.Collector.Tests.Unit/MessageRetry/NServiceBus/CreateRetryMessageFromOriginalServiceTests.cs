using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageRetry.NServiceBus;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry.NServiceBus
{
    public class CreateRetryMessageFromOriginalServiceTests
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

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Where(x => x.Key.StartsWith("$.diagnostics")).Select(x => x.Key).Should().BeEmpty();
        }

        [Test]
        public void ShouldRemoveExceptionHeaders()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.ExceptionInfo"))
                .Select(x => x.Key)
                .Should()
                .BeEmpty();
        }

        [Test]
        public void ShouldRemoveTimeoutHeaders()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.Timeout")).Select(x => x.Key).Should().BeEmpty();
        }

        [Test]
        public void ShouldRemoveRetryHeaders()
        {
            var errorHeaders = new List<string>
            {
                "NServiceBus.FLRetries",
                "NServiceBus.FailedQ",
                "NServiceBus.TimeOfFailure"
            };
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Select(x => x.Key).Intersect(errorHeaders).Should().BeEmpty();
            rawMessage.Headers.Where(x => x.Key.StartsWith("NServiceBus.Retries")).Select(x => x.Key).Should().BeEmpty();
        }

        [Test]
        public void ShouldRemoveProcessingHeaders()
        {
            var processingHeaders = new List<string>
            {
                "NServiceBus.ProcessingStarted",
                "NServiceBus.ProcessingEnded",
                "NServiceBus.ProcessingEndpoint",
                "NServiceBus.ProcessingMachine"
            };
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Select(x => x.Key).Intersect(processingHeaders).Should().BeEmpty();
        }

        [Test]
        public void ShouldNotRemoveOtherHeaders()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            rawMessage.Headers.Select(x => x.Key).Should().BeEquivalentTo("NServiceBus.MessageId",
                "NServiceBus.CorrelationId", "NServiceBus.MessageIntent", "NServiceBus.Version",
                "NServiceBus.TimeSent", "NServiceBus.ContentType", "NServiceBus.EnclosedMessageTypes",
                "WinIdName", "NServiceBus.ConversationId", "NServiceBus.OriginatingMachine",
                "NServiceBus.OriginatingEndpoint",
                "NServiceBus.RabbitMQ.CallbackQueue", "NServiceBus.ReplyToAddress");
        }
    }
}
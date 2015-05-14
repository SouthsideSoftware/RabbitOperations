using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry.NServiceBus
{
    public class CreateRetryMessageFromOriginalServiceTests
    {
        [Test]
        public void ShouldSetTimeSentHeaderToTimeOfRetry()
        {
            //arrange
            var utcNow = DateTime.UtcNow.AddSeconds(-1);
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            Helpers.ToUniversalDateTime(rawMessage.Headers[Headers.TimeSent]).Should().BeAfter(utcNow);
        }

        [Test]
        public void ShouldSetTimeSentHeaderToTimeOfRetryWhenOriginalIsMissingTimeSent()
        {
            //arrange
            var utcNow = DateTime.UtcNow.AddSeconds(-1);
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove(Headers.TimeSent);
            var creator = new CreateRetryMessageFromOriginalService();

            //act
            creator.PrepareMessageForRetry(rawMessage);

            //assert
            Helpers.ToUniversalDateTime(rawMessage.Headers[Headers.TimeSent]).Should().BeAfter(utcNow);
        }

        [Test]
        public void ShouldRemoveDiagnosticHeaders()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
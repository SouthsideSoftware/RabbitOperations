using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class CreateRawMessageFromMessageDocumentTests
    {
        [Test]
        public void CanProperlyCopyBodyFromDocument()
        {
            //arrange
            var messageDocument = new MessageDocument
            {
                Body = "This is a test"
            };

            //act
            var rawMessage = new RawMessage(messageDocument);

            //assert
            rawMessage.Body.Should().Be(messageDocument.Body);
        }

        [Test]
        public void CanProperlyCopyHeadersFromDocument()
        {
            //arrange
            var messageDocument = new MessageDocument
            {
                Headers = new Dictionary<string, string>
                {
                    {"one", "oneValue"},
                    {"two", "twoValue"}
                },
                Body = ""
            };

            //act
            var rawMessage = new RawMessage(messageDocument);

            //assert
            rawMessage.Headers.ShouldBeEquivalentTo(messageDocument.Headers);
        }
    }
}
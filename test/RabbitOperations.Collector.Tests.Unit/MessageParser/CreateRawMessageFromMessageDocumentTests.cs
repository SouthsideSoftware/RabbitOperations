using System.Collections.Generic;
using FluentAssertions;
using RabbitOperations.Domain;
using Xunit;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    public class CreateRawMessageFromMessageDocumentTests
    {
        [Fact]
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

        [Fact]
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
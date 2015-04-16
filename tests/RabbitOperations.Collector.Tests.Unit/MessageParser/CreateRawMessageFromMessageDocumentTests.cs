using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class CreateRawMessageFromMessageDocumentTests
    {
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
                }
            };

            //act
            var rawMessage = new RawMessage(messageDocument);

            //assert
            rawMessage.Headers.ShouldBeEquivalentTo(messageDocument.Headers);
        }

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
    }
}
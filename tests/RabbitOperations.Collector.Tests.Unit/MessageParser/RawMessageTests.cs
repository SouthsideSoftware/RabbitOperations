using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class RawMessageTests
    {
        [Test]
        public void CanReadAudit()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }

            //act
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            //assert
            rawMessage.Headers.Count.Should().BeGreaterThan(1);

        }

        [Test]
        public void CanReadError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }

            //act
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            //assert
            rawMessage.Headers.Count.Should().BeGreaterThan(1);

        }

        [Test]
        public void CanRoundTripBody()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();

            //act
            var publishData = rawMessage.GetEelementsForRabbitPublish();
            var newRawMessage = new RawMessage(new BasicDeliverEventArgs("tag", 1, false, "exchange", "", new BasicProperties{Headers = publishData.Item2}, publishData.Item1));

            //assert
            newRawMessage.Body.Should().Be(rawMessage.Body);
        }

        [Test]
        public void CanRoundTripHeaders()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();

            //act
            var publishData = rawMessage.GetEelementsForRabbitPublish();
            var newRawMessage = new RawMessage(new BasicDeliverEventArgs("tag", 1, false, "exchange", "", new BasicProperties { Headers = publishData.Item2 }, publishData.Item1));

            //assert
            newRawMessage.Headers.ShouldBeEquivalentTo(rawMessage.Headers);
        }

        [Test]
        public void HeadersAreDeepCopiedWhenConstructedFromMessageDocument()
        {
            //arrange
            var doc = new MessageDocument();
            var originalHeaders = new Dictionary<string, string>
            {
                {"foo1", "foo1" },
                {"foo2", "foo2" }
            };

            doc.Headers = originalHeaders;

            //act
            var rawMessage = new RawMessage(doc);
            rawMessage.Headers["foo1"] = "fi1";
            rawMessage.Headers.Remove("foo2");

            //assert
            rawMessage.Headers.Should().NotBeSameAs(originalHeaders, "The raw message dictionary is not the same as the message document dictionary");
            doc.Headers.Should().Equal(originalHeaders, "The original message's headers are unchanged");
        }
    }
}

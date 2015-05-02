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
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

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
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Audit.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            //act
            var publishData = rawMessage.GetEelementsForRabbitPublish();
            var newRawMessage = new RawMessage(new BasicDeliverEventArgs("tag", 1, false, "exchange", "", new BasicProperties { Headers = publishData.Item2 }, publishData.Item1));

            //assert
            newRawMessage.Headers.ShouldBeEquivalentTo(rawMessage.Headers);
        }
    }
}

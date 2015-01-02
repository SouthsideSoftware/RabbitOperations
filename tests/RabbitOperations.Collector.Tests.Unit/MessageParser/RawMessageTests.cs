using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RabbitOperations.Collector.MessageParser;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class RawMessageTests
    {
        [Test]
        public void ReadAudit()
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
        public void ReadError()
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
    }
}

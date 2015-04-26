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
    public class DetermineRetryDestinationTests
    {
        [Test]
        public void CanGetProperRetryDestinationFromError()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);

            var destinationFinder = new DetermineRetryDestination();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage);

            //assert
            destination.Should().Be("Autobahn.Configuration.Host");
        }
    }
}
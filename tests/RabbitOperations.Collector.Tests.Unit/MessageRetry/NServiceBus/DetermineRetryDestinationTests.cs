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
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().Be("Autobahn.Configuration.Host");
        }

        [Test]
        public void ReturnsProperRetryDestinationFromErrorWhenUserSuppliedIsWhitespace()
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
            var destination = destinationFinder.GetRetryDestination(rawMessage, "\t\t");

            //assert
            destination.Should().Be("Autobahn.Configuration.Host");
        }

        //test
        public void ReturnsUserSuppliedValueWhenNotNullOrWhitespace()
        {
            //arrange
            string userSupplied = "userQueue";
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            rawMessage.Headers.Remove("NServiceBus.FailedQ");

            var destinationFinder = new DetermineRetryDestination();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, userSupplied);

            //assert
            destination.Should().Be(userSupplied);    
        }

        [Test]
        public void ReturnsNullIfFailedQHeaderNotPresent()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            rawMessage.Headers.Remove("NServiceBus.FailedQ");

            var destinationFinder = new DetermineRetryDestination();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().BeNull();
        }

        [Test]
        public void ReturnsWholeQueueWhenDelimiterNotPresent()
        {
            //arrange
            string data;
            using (var reader = new StreamReader(Path.Combine("../../TestData", "Error.json")))
            {
                data = reader.ReadToEnd();
            }
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(data);
            rawMessage.Headers["NServiceBus.FailedQ"] = "simpleQueue";

            var destinationFinder = new DetermineRetryDestination();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().Be("simpleQueue");
        }
    }
}
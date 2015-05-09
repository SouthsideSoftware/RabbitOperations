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
    public class DetermineRetryDestinationServiceTests
    {
        [Test]
        public void CanGetProperRetryDestinationFromError()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var destinationFinder = new DetermineRetryDestinationService();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().Be("Autobahn.Configuration.Host");
        }

        [Test]
        public void ReturnsProperRetryDestinationFromErrorWhenUserSuppliedIsWhitespace()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var destinationFinder = new DetermineRetryDestinationService();

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
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.FailedQ");

            var destinationFinder = new DetermineRetryDestinationService();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, userSupplied);

            //assert
            destination.Should().Be(userSupplied);    
        }

        [Test]
        public void ReturnsNullIfFailedQHeaderNotPresent()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.FailedQ");

            var destinationFinder = new DetermineRetryDestinationService();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().BeNull();
        }

        [Test]
        public void ReturnsWholeQueueWhenDelimiterNotPresent()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers["NServiceBus.FailedQ"] = "simpleQueue";

            var destinationFinder = new DetermineRetryDestinationService();

            //act
            var destination = destinationFinder.GetRetryDestination(rawMessage, null);

            //assert
            destination.Should().Be("simpleQueue");
        }
    }
}
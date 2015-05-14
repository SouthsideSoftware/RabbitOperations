using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry.NServiceBus
{
    public class CreateBasicPropertiesServiceTests
    {
        [Test]
        public void ShouldSetContentTypeWhenNServiceBusHeaderExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.ContentType.Should().Be("application/json");
        }

        [Test]
        public void ShouldNotSetContentTypeWhenNServiceBusHeaderDoesNotExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.ContentType");

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.ContentType.Should().BeNull();
        }

        [Test]
        public void ShouldSetMessageIdWhenNServiceBusHeaderExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.MessageId.Should().Be("695742b4-58d0-4e3a-83a9-a4120116c48d");
        }

        [Test]
        public void ShouldNotSetMessageIdWhenNServiceBusHeaderDoesNotExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.MessageId");

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.MessageId.Should().BeNull();
        }

        [Test]
        public void ShouldSetCorrelationIdWhenNServiceBusHeaderExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.CorrelationId.Should().Be("695742b4-58d0-4e3a-83a9-a4120116c48d");
        }

        [Test]
        public void ShouldNotSetCorrelationIdWhenNServiceBusHeaderDoesNotExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.CorrelationId");

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.CorrelationId.Should().BeNull();
        }

        [Test]
        public void ShouldSetTypeWhenNServiceBusHeaderExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.Type.Should().Be("Autobahn.Configurations.Contracts.Commands.ValidateConfigurations");
        }

        [Test]
        public void ShouldNotSetTypeWhenNServiceBusHeaderDoesNotExists()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Remove("NServiceBus.EnclosedMessageTypes");

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.Type.Should().BeNull();
        }

        [Test]
        public void ShouldNotSetDeliveryModeToPersistent()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            byte persistentDeliverMode = 2;

            var service = new CreateBasicPropertiesService();

            //act
            var properties = service.Create(rawMessage);

            //assert
            properties.DeliveryMode.Should().Be(persistentDeliverMode);
        }
    }
}
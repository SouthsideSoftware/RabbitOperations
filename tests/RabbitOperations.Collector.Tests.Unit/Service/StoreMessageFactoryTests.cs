using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.CastleWindsor;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class StoreMessageFactoryTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ServiceLocator.Container.Install(new Installer());
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ServiceLocator.ClearContainer();
        }

        [Test]
        public void ShouldCreateMessageStoreServiceForRetry()
        {
            //arrange
            var factory = new StoreMessagesFactory();

            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(Headers.Retry, "test");

            //act
            var service = factory.MessageStorageServiceFor(rawMessage);

            //assert
            service.Should().BeOfType<StoreMessagesThatAreRetriesService>();
        }

        [Test]
        public void ShouldCreateMessageStoreServiceForNonRetry()
        {
            //arrange
            var factory = new StoreMessagesFactory();

            var rawMessage = MessageTestHelpers.GetErrorMessage();

            //act
            var service = factory.MessageStorageServiceFor(rawMessage);

            //assert
            service.Should().BeOfType<StoreMessagesThatAreNotRetriesService>();
        }
    }
}

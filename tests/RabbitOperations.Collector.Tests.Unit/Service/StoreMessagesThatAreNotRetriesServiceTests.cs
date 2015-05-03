using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class StoreMessagesThatAreNotRetriesServiceTests : RavenDbTest
    {
        [TestFixtureSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void ShouldReturnResultWithMeaningfulAdditionalInfoWhenOriginalMessageMissing()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> {1}
            });

            //assert
            result.RetryMessageItems.First()
                .AdditionalInfo.Should()
                .ContainEquivalentOf("original message does not exist");
        }

        [Test]
        public void ShouldReturnResultIndicatingNoRetryWhenOriginalMessageMissing()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { 1 }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }
    }
}
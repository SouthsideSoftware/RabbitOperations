using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.MessageRetry;
using Raven.Abstractions.Data;
using SouthsideUtility.RavenDB.Testing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain;
using Raven.Client;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
    [TestFixture]
    public class RetryMessagesServiceTests : RavenDbTest
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
                RetryIds = new List<long> {-1}
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
                RetryIds = new List<long> { -1 }
            });

            //assert
            result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
        }

        [Test]
        public void ShouldChangeStatusOfOriginalMessageToRetryPendingWhenRetryStarts()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            var service = fixture.Create<RetryMessagesService>();

            var originalMessge = new MessageDocument();
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                session.Store(originalMessge);
                session.SaveChanges();
            }

            //act
            var result = service.Retry(new RetryMessageModel
            {
                RetryIds = new List<long> { originalMessge.Id }
            });

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var message = session.Load<MessageDocument>(originalMessge.Id);
                message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
            }
        }
    }
}

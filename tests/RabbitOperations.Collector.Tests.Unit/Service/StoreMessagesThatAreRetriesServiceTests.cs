using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageParser.NServiceBus;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class StoreMessagesThatAreRetriesServiceTests : RavenDbTest
    {
        [TestFixtureSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void ShouldStoreErrorWithProperErrorStatus()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();
            rawMessage.Headers.Add(AddRetryTrackingHeadersService.RetryHeader, "test");

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreNotRetriesService>();

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration {ApplicationId = "test"}));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry, "AdditionalErrorStatus should be IsRetry");
                doc.IsError.Should().BeTrue("IsError should be true");
                doc.CanRetry.Should().BeFalse("Can retry should be true");
            }
        }

        [Test]
        public void ShouldStoreAuditWithProperErrorStatus()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();
            rawMessage.Headers.Add(AddRetryTrackingHeadersService.RetryHeader, "test");

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreNotRetriesService>();

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" }));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                doc.Should().NotBeNull("Stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.IsRetry, "AdditionalErrorStatus should be IsRetry");
                doc.IsError.Should().BeFalse("IsError should be false");
                doc.CanRetry.Should().BeFalse("Can retry should be true");
            }
        }
    }
}
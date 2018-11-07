using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.Service;
using RabbitOperations.Domain;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class StoreMessagesThatAreNotRetriesServiceTests : RavenDbTest
    {
        [OneTimeSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void ShouldStoreErrorWithProperErrorStatus()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

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
                doc.Should().NotBeNull("stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.Unresolved, "Additional error status should be Unresolved");
                doc.IsError.Should().BeTrue("IsError should be true");
                doc.CanRetry.Should().BeTrue("CanRetry should be true");
            }
        }

        [Test]
        public void ShouldStoreErrorWithProperExpiration()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetErrorMessage();

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreNotRetriesService>();

            int auditExpirationHours = 1;
            int errorExpirationHours = 2;

            DateTime expiresAfter = DateTime.UtcNow.AddHours(errorExpirationHours).AddMinutes(-15);
            DateTime expiresBefore = DateTime.UtcNow.AddHours(errorExpirationHours).AddMinutes(15);

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test" , DocumentExpirationInHours = auditExpirationHours, ErrorDocumentExpirationInHours = errorExpirationHours}));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                var expires = DateTime.Parse(session.Advanced.GetMetadataFor(doc)["Raven-Expiration-Date"].ToString());
                expires.Should().BeAfter(expiresAfter, $"Should expire after around {errorExpirationHours} hours");
                expires.Should().BeBefore(expiresBefore, $"Should expire after around {errorExpirationHours} hours");
            }
        }

        [Test]
        public void ShouldStoreAuditWithProperErrorStatus()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();

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
                doc.Should().NotBeNull("stored document should not be null");
                doc.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.NotAnError, "additional error status should be NotAnError");
                doc.IsError.Should().BeFalse("IsError should be false");
                doc.CanRetry.Should().BeFalse("CanRetry should be false");
            }
        }

        [Test]
        public void ShouldStoreAuditWithProperExpiration()
        {
            //arrange
            var rawMessage = MessageTestHelpers.GetAuditMessage();

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Register(() => Store);
            fixture.Register<IHeaderParser>(() => new HeaderParser());
            var service = fixture.Create<StoreMessagesThatAreNotRetriesService>();

            int auditExpirationHours = 1;
            int errorExpirationHours = 2;

            DateTime expiresAfter = DateTime.UtcNow.AddHours(auditExpirationHours).AddMinutes(-15);
            DateTime expiresBefore = DateTime.UtcNow.AddHours(auditExpirationHours).AddMinutes(15);

            //act
            var id = service.Store(rawMessage,
                new QueueSettings("test", new ApplicationConfiguration { ApplicationId = "test", DocumentExpirationInHours = auditExpirationHours, ErrorDocumentExpirationInHours = errorExpirationHours }));

            //assert
            using (var session = Store.OpenSessionForDefaultTenant())
            {
                var doc = session.Load<MessageDocument>(id);
                var expires = DateTime.Parse(session.Advanced.GetMetadataFor(doc)["Raven-Expiration-Date"].ToString());
                expires.Should().BeAfter(expiresAfter, $"Should expire after around {auditExpirationHours} hours");
                expires.Should().BeBefore(expiresBefore, $"Should expire after around {auditExpirationHours} hours");
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.MessageBusTechnologies.NServiceBus;
using RabbitOperations.Collector.MessageRetry;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain;
using SouthsideUtility.RavenDB.Testing;
using Headers = RabbitOperations.Collector.MessageBusTechnologies.Common.Headers;

namespace RabbitOperations.Collector.Tests.Unit.MessageRetry
{
	[TestFixture]
	public class RetryMessagesServiceDestinationTests : RavenDbTest
	{
		[OneTimeSetUp]
		public void TestFixtireSetup()
		{
			new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
		}

		[Test]
		public void ShouldReturnOneDestinationWhenThereIsOneMessage()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			fixture.Register<IDetermineRetryDestination>(() => new DetermineRetryDestinationService());
			var service = fixture.Create<RetryMessagesService>();

			var rawMessage = MessageTestHelpers.GetErrorMessage();
			var originalMessge = new MessageDocument();
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.GetRetryDestinations(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id }
			});

			//assert
			result.RetryDestinations.Count.Should().Be(1);
		}

		[Test]
		public void ShouldReturnOneDestinationWhenThereAreManyMessagesWithSameDestination()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			fixture.Register<IDetermineRetryDestination>(() => new DetermineRetryDestinationService());
			var service = fixture.Create<RetryMessagesService>();

			var rawMessage = MessageTestHelpers.GetErrorMessage();
			var originalMessge = new MessageDocument();
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			var originalMessge2 = new MessageDocument();
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge2);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge2);
				session.SaveChanges();
			}

			//act
			var result = service.GetRetryDestinations(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id, originalMessge2.Id }
			});

			//assert
			result.RetryDestinations.Count.Should().Be(1);
		}

		[Test]
		public void ShouldReturnMultipleDestinatiosnWhenThereAreManyMessagesWithDifferntDestinations()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			fixture.Register<IDetermineRetryDestination>(() => new DetermineRetryDestinationService());
			var service = fixture.Create<RetryMessagesService>();

			var rawMessage = MessageTestHelpers.GetErrorMessage();
			var originalMessge = new MessageDocument();
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			rawMessage = MessageTestHelpers.GetAuditMessage();
			var originalMessge2 = new MessageDocument();
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge2);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge2);
				session.SaveChanges();
			}

			//act
			var result = service.GetRetryDestinations(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id, originalMessge2.Id }
			});

			//assert
			result.RetryDestinations.Count.Should().BeGreaterThan(1);
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
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
	public class RetryMessagesServiceTestsWithForceSet : RavenDbTest
	{
		[OneTimeSetUp]
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
				RetryIds = new List<long> {-1},
				ForceRetry = true
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
				RetryIds = new List<long> { -1 },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().IsRetrying.Should().BeFalse();
		}

		[Test]
		public void ShouldChangeStatusOfOriginalNonErrorMessageToRetryPendingWhenRetryStarts()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				var message = session.Load<MessageDocument>(originalMessge.Id);
				message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
			}
		}

		[Test]
		public void ShouldRetryWhenOriginalMessageIsResolvedAndForceSet()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Resolved;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().IsRetrying.Should().BeTrue();
		}

		[Test]
		public void ShouldRetryWhenOriginalMessageIsNotAnErrorAndForceSet()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().IsRetrying.Should().BeTrue();
		}

		[Test]
		public void ShouldRetryWhenOriginalMessageIsClosedAndForceSet()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.Closed;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().IsRetrying.Should().BeTrue();
		}

		[Test]
		public void ShouldRetryWhenOriginalMessageIsRetryPendingAndForceSet()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.RetryPending;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().IsRetrying.Should().BeTrue();
		}

		[Test]
		public void ShouldRetainAllHeadersOfOriginalMessageAfterForcedRetry()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				var message = session.Load<MessageDocument>(originalMessge.Id);
				message.AdditionalErrorStatus.Should().Be(AdditionalErrorStatus.RetryPending);
			}
		}

		[Test]
		public void ShouldReturnAdditionalErrorStatusStringRetryPendingWhenForcedRetryWorks()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			fixture.Register<ICreateRetryMessagesFromOriginal>(() => new CreateRetryMessageFromOriginalService());
			var service = fixture.Create<RetryMessagesService>();

			var rawMessage = MessageTestHelpers.GetErrorMessage();
			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
			new HeaderParser().AddHeaderInformation(rawMessage, originalMessge);
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				var message = session.Load<MessageDocument>(originalMessge.Id);
				AssertRetriedMessageHeadersExcludingRetryIdSameAsOriginal(message, originalMessge);
			}
		}

		private static void AssertRetriedMessageHeadersExcludingRetryIdSameAsOriginal(MessageDocument message,
			MessageDocument originalMessge)
		{
			message.Headers.Remove(Headers.Retry);
			message.Headers.Should().BeEquivalentTo(originalMessge.Headers);
		}

		[Test]
		public void ShouldReturnCanRetryFalseWhenItWorks()
		{
			//arrange
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			fixture.Register(() => Store);
			var service = fixture.Create<RetryMessagesService>();

			var originalMessge = new MessageDocument();
			originalMessge.AdditionalErrorStatus = AdditionalErrorStatus.NotAnError;
			using (var session = Store.OpenSessionForDefaultTenant())
			{
				session.Store(originalMessge);
				session.SaveChanges();
			}

			//act
			var result = service.Retry(new RetryMessageModel
			{
				RetryIds = new List<long> { originalMessge.Id },
				ForceRetry = true
			});

			//assert
			result.RetryMessageItems.First().CanRetryOriginalMessage.Should().BeFalse();
		}
	}
}

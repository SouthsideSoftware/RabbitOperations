using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Configuration
{
    [TestFixture]
    public class SettingsTests : RavenDbTest
    {
        [TestFixtureSetUp]
        public void TestFixtireSetup()
        {
            new RavenTenantInitializer(Store).InitializeTenant(Settings.StaticDefaultRavenDBTenant);
        }

        [Test]
        public void SettingsCanBeSavedAndLoaded()
        {
            //arrange
            var settings = new Settings(Store);
            settings.AuditQueue = "xxx";
            settings.Save();

            //act
            settings = new Settings(Store);

            //assert 
            settings.AuditQueue.Should().Be("xxx");
        }

        [Test]
        public void CanGetMessageTypeHandlerForATypeWhenOneMatches()
        {
            //arrange
            var settings = new Settings(Store);
            settings.MessageHandlingInstructions = new List<MessageTypeHandling>
            {
                new MessageTypeHandling
                {
                    MessageTypes = new List<string>
                    {
                        "a",
                        "b"
                    },
                    Keywords = new List<string>
                    {
                        "one"
                    }
                },
                new MessageTypeHandling
                {
                    MessageTypes = new List<string>
                    {
                        "c",
                        "d"
                    },
                    Keywords = new List<string>
                    {
                        "two"
                    }
                }
            };

            //act
            var handling = settings.MessageTypeHandlingFor("c");

            //assert
            handling.Keywords[0].Should().Be("two");
        }

        [Test]
        public void ShouldReturnFirstHandlerThatHasMatchingTypeWhenMultiplesMatch()
        {
            //arrange
            var settings = new Settings(Store);
            settings.MessageHandlingInstructions = new List<MessageTypeHandling>
            {
                new MessageTypeHandling
                {
                    MessageTypes = new List<string>
                    {
                        "a",
                        "c"
                    },
                    Keywords = new List<string>
                    {
                        "one"
                    }
                },
                new MessageTypeHandling
                {
                    MessageTypes = new List<string>
                    {
                        "c",
                        "d"
                    },
                    Keywords = new List<string>
                    {
                        "two"
                    }
                }
            };

            //act
            var handling = settings.MessageTypeHandlingFor("c");

            //assert
            handling.Keywords[0].Should().Be("one");
        }
    }
}

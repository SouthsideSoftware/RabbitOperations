using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Configuration
{
    [TestFixture]
    public class SettingsTests : RavenDbTest
    {
        public SettingsTests() : base(ravenInMemory: true) { }

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
            const string environmentId = "one";
            settings.Environments.Add(new ApplicationConfiguration
            {
                AuditQueue = "xxx",
                ApplicationId = environmentId
            });
            settings.Save();

            //act
            settings = new Settings(Store);

            //assert 
            settings.Environments.First(x => x.ApplicationId == environmentId).AuditQueue.Should().Be("xxx");
        }

        [Test]
        public void CanGetMessageTypeHandlerForATypeWhenOneMatches()
        {
            //arrange
            var settings = new Settings(Store);
            settings.GlobalMessageHandlingInstructions = new List<MessageTypeHandling>
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
            settings.GlobalMessageHandlingInstructions = new List<MessageTypeHandling>
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

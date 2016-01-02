using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Domain.Configuration;
using Xunit;
using SouthsideUtility.Core.DesignByContract;
using RabbitOperations.Collector.Configuration;
using Microsoft.Extensions.OptionsModel;
using Moq;
using RabbitOperations.Collector.Tests.Unit.RavenDb;

namespace RabbitOperations.Collector.Tests.Unit.Configuration
{
    public class SettingsTests : IClassFixture<RavenDbInMemory>
    {
        private readonly RavenDbInMemory ravenDb;

        public SettingsTests(RavenDbInMemory ravenDb)
        {
            Verify.RequireNotNull(ravenDb, "ravenDb");
            this.ravenDb = ravenDb;
        }

        [Fact]
        public void SettingsCanBeSavedAndLoaded()
        {
            //arrange
            var mockSettings = Mock.Of<IOptions<AppSettings>>();
            var settings = new Settings(ravenDb.Store, mockSettings);
            const string applicationId = "one";
            //settings.Applications.Add(new ApplicationConfiguration
            //{
            //    AuditQueue = "xxx",
            //    ApplicationId = applicationId
            //});
            //settings.Save();

            ////act
            //settings = new Settings(Store);

            ////assert 
            //settings.Applications.First(x => x.ApplicationId == applicationId).AuditQueue.Should().Be("xxx");
        }

        //[Test]
        //public void CanGetMessageTypeHandlerForATypeWhenOneMatches()
        //{
        //    //arrange
        //    var settings = new Settings(Store);
        //    settings.GlobalMessageHandlingInstructions = new List<MessageTypeHandling>
        //    {
        //        new MessageTypeHandling
        //        {
        //            MessageTypes = new List<string>
        //            {
        //                "a",
        //                "b"
        //            },
        //            Keywords = new List<string>
        //            {
        //                "one"
        //            }
        //        },
        //        new MessageTypeHandling
        //        {
        //            MessageTypes = new List<string>
        //            {
        //                "c",
        //                "d"
        //            },
        //            Keywords = new List<string>
        //            {
        //                "two"
        //            }
        //        }
        //    };

        //    //act
        //    var handling = settings.MessageTypeHandlingFor("c");

        //    //assert
        //    handling.Keywords[0].Should().Be("two");
        //}

        //[Test]
        //public void ShouldReturnFirstHandlerThatHasMatchingTypeWhenMultiplesMatch()
        //{
        //    //arrange
        //    var settings = new Settings(Store);
        //    settings.GlobalMessageHandlingInstructions = new List<MessageTypeHandling>
        //    {
        //        new MessageTypeHandling
        //        {
        //            MessageTypes = new List<string>
        //            {
        //                "a",
        //                "c"
        //            },
        //            Keywords = new List<string>
        //            {
        //                "one"
        //            }
        //        },
        //        new MessageTypeHandling
        //        {
        //            MessageTypes = new List<string>
        //            {
        //                "c",
        //                "d"
        //            },
        //            Keywords = new List<string>
        //            {
        //                "two"
        //            }
        //        }
        //    };

        //    //act
        //    var handling = settings.MessageTypeHandlingFor("c");

        //    //assert
        //    handling.Keywords[0].Should().Be("one");
        //}
    }
}

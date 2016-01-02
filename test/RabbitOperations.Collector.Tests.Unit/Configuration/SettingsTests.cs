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
using FluentAssertions;

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
            Settings settings = CreateSettings();
            const string applicationId = "one";
            settings.Applications.Add(new ApplicationConfiguration
            {
                AuditQueue = "xxx",
                ApplicationId = applicationId
            });
            settings.Save();

            //act
            settings = CreateSettings();

            //assert 
            settings.Applications.First(x => x.ApplicationId == applicationId).AuditQueue.Should().Be("xxx");
        }

        private Settings CreateSettings()
        {
            var mockSettings = Mock.Of<IOptions<AppSettings>>();
            return new Settings(ravenDb.Store, mockSettings);
        }
    }
}

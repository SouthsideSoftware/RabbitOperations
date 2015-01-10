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
using SouthsideUtility.RavenDB.Testing;

namespace RabbitOperations.Collector.Tests.Unit.Configuration
{
    [TestFixture]
    public class SettingsTests : RavenDbTest
    {
        [Test]
        public void QuickTest()
        {
            //arrange
            var settings = new Settings(Store);

            //act
            var audit = settings.AuditQueue;

            //assert 
            audit.Should().Be("audit");
        }
    }
}

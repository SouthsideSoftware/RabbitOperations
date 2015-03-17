using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using FluentAssertions;
using RabbitOperations.Collector.Configuration.Interfaces;

namespace RabbitOperations.Collector.Tests.Unit.Web.LinkHelper
{
    [TestFixture]
    public class LinkHelperTests
    {
        [Test]
        public void ReturnsProperUrlForEmbedded()
        {
            //arrange
            var settings = Mock.Of<ISettings>(x => x.EmbedRavenDB == true && x.EmbeddedRavenDBManagementPort == 6);
            
            //act
            var url = RabbitOperations.Collector.Web.Helpers.LinkHelper.RavenStudioUrl(settings, "foo");

            //assert
            url.Should().Be("http://foo:6");
        }

        [Test]
        public void ReturnsProperUrlForExternal()
        {
            //arrange
            var settings = Mock.Of<ISettings>(x => x.EmbedRavenDB == false 
                && x.EmbeddedRavenDBManagementPort == 6
                && x.RavenDBConnectionString == "Url = http://host:99");

            //act
            var url = RabbitOperations.Collector.Web.Helpers.LinkHelper.RavenStudioUrl(settings, "foo");

            //assert
            url.Should().Be("http://host:99");
        }
    }
}

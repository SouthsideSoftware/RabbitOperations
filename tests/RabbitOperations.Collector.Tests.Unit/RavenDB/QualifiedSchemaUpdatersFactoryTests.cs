using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB;
using RabbitOperations.Collector.RavenDB.Interfaces;

namespace RabbitOperations.Collector.Tests.Unit.RavenDB
{
    [TestFixture]
    public class QualifiedSchemaUpdatersFactoryTests
    {
        [Test]
        public void ShouldReturnEmptyCollectionWhenThereAreNoUpdaters()
        {
            //arrange
            var settingsStub = Mock.Of<ISettings>(m => m.DatabaseSchemaVersion == 0);
            var factory = new QualifiedSchemaUpdatersFactory(new List<IUpdateSchemaVersion>(), settingsStub);

            //act
            var updaters = factory.Get();

            //assert 
            updaters.Count.Should().Be(0);
        }
    }
}

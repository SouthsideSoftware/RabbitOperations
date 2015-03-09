using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void ShouldReturnAllUpdatersWithVersionGreaterThanCurrent()
        {
            //arrange
            var settingsStub = Mock.Of<ISettings>(m => m.DatabaseSchemaVersion == 2);
            var factory = new QualifiedSchemaUpdatersFactory(new List<IUpdateSchemaVersion>
            {
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 1),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 2),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 3),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 4)
            }, settingsStub);

            //act
            var updaters = factory.Get();

            //assert 
            updaters.Select(x => x.SchemaVersion).Should().BeEquivalentTo(new List<int> {3, 4});
        }

        [Test]
        public void ShouldReturnQualifiedUpdatersSortedByVersionNumber()
        {
            //arrange
            var settingsStub = Mock.Of<ISettings>(m => m.DatabaseSchemaVersion == 1);
            var factory = new QualifiedSchemaUpdatersFactory(new List<IUpdateSchemaVersion>
            {
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 4),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 2),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 3),
                Mock.Of<IUpdateSchemaVersion>(m => m.SchemaVersion == 1)
            }, settingsStub);

            //act
            var updaters = factory.Get();

            //assert 
            updaters.Select(x => x.SchemaVersion).Should().Equal(new List<int> { 2, 3, 4 });
        }
    }
}
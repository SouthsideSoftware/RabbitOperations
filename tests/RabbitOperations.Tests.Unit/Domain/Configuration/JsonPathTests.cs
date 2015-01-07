using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Core.DesignByContract.Exceptions;

namespace RabbitOperations.Tests.Unit.Domain.Configuration
{
    [TestFixture]
    public class JsonPathTests
    {
        [Test]
        public void CannotConstructWithEmptyString()
        {
            //arrange
            Action act = () => new JsonPath("");

            //act and assert
            act.ShouldThrow<PreconditionException>();
        }

        [Test]
        public void ShouldHaveOnePartWhenPathHasNoDots()
        {
            //act
            var path = new JsonPath("Item");

            //assert
            path.Parts.Should().HaveCount(1);
        }

        [Test]
        public void ShouldSplitIntoPartsOnDots()
        {
            //act
            var path = new JsonPath("Item1.Item2.Item3");

            //assert
            path.Parts.Should().Equal(new List<string> {"Item1", "Item2", "Item3"});
        }

        [Test]
        public void ShoulNotHaveEmptyLastPartIfEndsWithDot()
        {
            //act
            var path = new JsonPath("Item1.Item2.");

            //assert
            path.Parts.Should().Equal(new List<string> { "Item1", "Item2"});
        }

        [Test]
        public void ShoulTrimParts()
        {
            //act
            var path = new JsonPath("  Item1 .\tItem2.");

            //assert
            path.Parts.Should().Equal(new List<string> { "Item1", "Item2" });
        }

        [Test]
        public void ShouldReturnDottedPathFromToString()
        {
            //act
            var path = new JsonPath("Item1.Item2.Item3");

            //assert
            path.ToString().Should().Be("Item1.Item2.Item3");
        }
    }
}

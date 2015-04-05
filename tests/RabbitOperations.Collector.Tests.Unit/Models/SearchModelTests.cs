using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.Models;

namespace RabbitOperations.Collector.Tests.Unit.Models
{
    [TestFixture]
    public class SearchModelTests
    {
        [Test]
        public void GetsEmptyRavenSearchSringOnNullSearchString()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SearchString = null
            };

            //act and assert
            searchModel.RavenSearchString.Should().BeEmpty();
        }

        [Test]
        public void GetsEmptyRavenSearchSringOnEmptySearchString()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SearchString = ""
            };

            //act and assert
            searchModel.RavenSearchString.Should().BeEmpty();
        }

        [Test]
        public void GetsEmptyRavenSearchSringSearchStringIsJSUndefined()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SearchString = "undefined"
            };

            //act and assert
            searchModel.RavenSearchString.Should().BeEmpty();
        }

        [Test]
        public void GetsTimeSentDescRavenSortWhenSortFieldIsNull()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SortField = null
            };

            //act and assert
            searchModel.RavenSort.Should().Be("-TimeSent");
        }

        [Test]
        public void GetsTimeSentDescRavenSortWhenSortFieldIsEmpty()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SortField = ""
            };

            //act and assert
            searchModel.RavenSort.Should().Be("-TimeSent");
        }

        [Test]
        public void GetsProperRavenSortWhenSortFieldIsSpecifiedForAscendingSort()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SortField = "Field",
                SortAscending = true
            };

            //act and assert
            searchModel.RavenSort.Should().Be("+Field");
        }

        [Test]
        public void GetsProperRavenSortWhenSortFieldIsSpecifiedForDescendingSort()
        {
            //arrange
            var searchModel = new SearchModel
            {
                SortField = "Field",
                SortAscending = false
            };

            //act and assert
            searchModel.RavenSort.Should().Be("-Field");
        }
    }
}

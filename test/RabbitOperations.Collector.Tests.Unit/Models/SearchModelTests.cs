using FluentAssertions;
using RabbitOperations.Collector.Models;
using Xunit;

namespace RabbitOperations.Collector.Tests.Unit.Models
{
    public class SearchModelTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
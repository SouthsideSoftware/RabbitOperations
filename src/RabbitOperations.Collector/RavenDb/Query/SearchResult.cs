using System.Collections.Generic;
using RabbitOperations.Collector.Models;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDb.Query
{
    public class SearchResult<T>
    {
        public SearchResult(SearchModel searchModel, IList<T> results, RavenQueryStatistics stats)
        {
            Verify.RequireNotNull(results, "results");
            Verify.RequireNotNull(stats, "stats");
            Verify.RequireNotNull(searchModel, "searchModel");

            SearchModel = searchModel;
            TotalResults = stats.TotalResults;
            TimingsInMilliseconds = stats.TimingsInMilliseconds;
            Results = results;
        }

        public SearchModel SearchModel { get; private set; }
        public int TotalResults { get; private set; }
        public Dictionary<string, double> TimingsInMilliseconds { get; private set; }
        public IList<T> Results { get; private set; }

    }
}
using System.Collections.Generic;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.Query
{
    public class SearchResult<T>
    {
        public SearchResult(string searchString, int page, IList<T> results, RavenQueryStatistics stats)
        {
            Verify.RequireNotNull(results, "results");
            Verify.RequireNotNull(stats, "stats");

            SearchString = searchString;
            TotalResults = stats.TotalResults;
            TimingsInMilliseconds = stats.TimingsInMilliseconds;
            Results = results;
            Page = page;
        } 

        public string SearchString { get; private set; }
        public int TotalResults { get; private set; }
        public Dictionary<string, double> TimingsInMilliseconds { get; private set; }
        public IList<T> Results { get; private set; }
        public int Page { get; private set; }

    }
}
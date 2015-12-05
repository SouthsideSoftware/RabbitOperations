using System.Linq;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.Query
{
    public class BasicSearch : IBasicSearch
    {
        private readonly IDocumentStore store;

        public BasicSearch(IDocumentStore store)
        {
            Verify.RequireNotNull(store, "store");

            this.store = store;
        }

        public SearchResult<MessageSearchResult> Search(SearchModel searchModel)
        {
            using (var session = store.OpenSessionForDefaultTenant())
            {
                RavenQueryStatistics stats = null;
                var results = session.Advanced.DocumentQuery<MessageSearchResult, MessageDocument_Search>()
                    .Where(searchModel.RavenSearchString)
                    .OrderBy(searchModel.RavenSort)
                    .SelectFields<MessageSearchResult>()
                    .UsingDefaultField("Any")
                    .Skip(searchModel.Page*searchModel.Take)
                    .Take(searchModel.Take)
                    .Statistics(out stats)
                    .ShowTimings()
                    .ToList();

                Log.Debug(
                    "Query for {SearchCriteria} order by {SortOrder} matched {TotalCount} documents.  Took {PageCount} from page {Page}.  Timings: {Timings}",
                    !string.IsNullOrWhiteSpace(searchModel.RavenSearchString)
                        ? searchModel.RavenSearchString
                        : "ALL DOCS", searchModel.RavenSort, stats.TotalResults, searchModel.Take, searchModel.Page + 1,
                    stats.TimingsInMilliseconds);

                return new SearchResult<MessageSearchResult>(searchModel, results, stats);
            }
        }

        public MessageDocument Get(int id)
        {
            using (var session = store.OpenSessionForDefaultTenant())
            {
                return session.Load<MessageDocument>(id);
            }
        }
    }
}
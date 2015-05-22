using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using NLog;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Collector.Web.Modules.Api.V1;
using RabbitOperations.Domain;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.Query
{
    public class BasicSearch : IBasicSearch
    {
        private readonly IDocumentStore store;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

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
                        .UsingDefaultField("Any").Skip(searchModel.Page * searchModel.Take).Take(searchModel.Take).Statistics(out stats).ShowTimings()
                        .ToList();
                if (logger.IsDebugEnabled)
                {
                    var timings = string.Join("\n\t",
                        stats.TimingsInMilliseconds.Select(x => string.Format("{0}:{1}ms", x.Key, x.Value)));
                    logger.Debug(
                        "Query for {0} order by {1} matched {2} documents.  Took {3} from page (0-based) {4}.  Timings: \n\t{5}",
                        !string.IsNullOrWhiteSpace(searchModel.RavenSearchString) ? searchModel.RavenSearchString : "ALL DOCS", searchModel.RavenSort, stats.TotalResults, searchModel.Take, searchModel.Page, timings);
                }

                return new SearchResult<MessageSearchResult>(searchModel, results, stats);
            }
        }
    }
}

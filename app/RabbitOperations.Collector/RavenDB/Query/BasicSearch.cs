using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using NLog;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Domain;
using Raven.Client;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.Query
{
    public class BasicSearch : IBasicSearch
    {
        private readonly IDocumentStore store;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public BasicSearch(IDocumentStore store)
        {
            Verify.RequireNotNull(store, "store");

            this.store = store;
        }

        public SearchResult<MessageDocument> Search(string searchString, int take, int page)
        {
            using (var session = store.OpenSessionForDefaultTenant())
            {
                RavenQueryStatistics stats = null;
                var results = session.Advanced.DocumentQuery<MessageDocument, MessageDocument_Search>()
                        .Where(searchString)
                        .OrderByDescending(x => x.TimeSent)
                        .UsingDefaultField("Any").Skip(take * page).Take(take).Statistics(out stats).ShowTimings()
                        .ToList();
                if (logger.IsDebugEnabled)
                {
                    var timings = string.Join("\n\t",
                        stats.TimingsInMilliseconds.Select(x => string.Format("{0}:{1}ms", x.Key, x.Value)));
                    logger.Debug(
                        "Query for {0} matched {1} documents.  Took {2} from page (0-based) {3}.  Timings: \n\t{4}",
                        searchString, stats.TotalResults, take, page, timings);
                }

                return new SearchResult<MessageDocument>(searchString, page, results, stats);
            }
        }
    }
}

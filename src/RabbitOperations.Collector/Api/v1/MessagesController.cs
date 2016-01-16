using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDb.Query;
using RabbitOperations.Collector.RavenDb.Query.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RabbitOperations.Collector.Api.v1
{
    [Route("api/[controller]", Name = "defaultMessages")]
    [Route("api/v1/[controller]", Name = "version1Messages")]
    public class MessagesController : Controller
    {
        private readonly IBasicSearch basicSearch;

        public MessagesController(IBasicSearch basicSearch)
        {
            Verify.RequireNotNull(basicSearch, "basicSearch");

            this.basicSearch = basicSearch;
        }

        // GET: api/values
        [HttpGet]
        public SearchResult<MessageSearchResult> Get(SearchModel searchModel)
        {
            Verify.RequireNotNull(searchModel, "searchModel");

            return basicSearch.Search(searchModel);
        }
    }
}

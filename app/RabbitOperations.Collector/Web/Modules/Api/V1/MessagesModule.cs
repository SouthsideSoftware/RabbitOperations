using Nancy;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class MessagesModule : NancyModule
    {
        private readonly IBasicSearch basicSearch;

        public MessagesModule(IBasicSearch basicSearch) : base("Api/V1/Messages")
        {
            Verify.RequireNotNull(basicSearch, "basicSearch");

            this.basicSearch = basicSearch;

            Get["/{searchString?undefined}"] = parameters =>
            {
                return basicSearch.Search(parameters.searchString, 100, 0);
            };
        }
    }
}
using Nancy;
using Nancy.ModelBinding;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class MessagesModule : NancyModule
    {
        private readonly IBasicSearch basicSearch;
        private readonly IRetryMessages retryMessagesService;

        public MessagesModule(IBasicSearch basicSearch, IRetryMessages retryMessagesService) : base("Api/V1/Messages")
        {
            Verify.RequireNotNull(basicSearch, "basicSearch");
            Verify.RequireNotNull(retryMessagesService, "retryMessagesService");

            this.basicSearch = basicSearch;
            this.retryMessagesService = retryMessagesService;

            Get["/{searchString?undefined}"] = parameters =>
            {
                SearchModel searchModel = this.Bind<SearchModel>();
                return basicSearch.Search(searchModel);
            };

            Put["/retry"] = parameters =>
            {
                var retryModel = this.Bind<RetryMessageModel>();
                return retryMessagesService.Retry(retryModel);
            };
        }
    }
}
using System.IO;
using System.Text;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Models;
using RabbitOperations.Collector.RavenDB.Query.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
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

            Get["/Search/{searchString?undefined}"] = parameters =>
            {
                SearchModel searchModel = this.Bind<SearchModel>();
                return basicSearch.Search(searchModel);
            };

            Put["/retry"] = parameters =>
            {
                var retryModel = this.Bind<RetryMessageModel>();
                return retryMessagesService.Retry(retryModel);
            };

            Get["/{id}"] = parameters => {
				var serializer = new JsonSerializer();
	            var data = JObject.FromObject(basicSearch.Get(parameters.id), serializer).ToString();
	            byte [] bytes = Encoding.UTF8.GetBytes(data);

				return new Response
	            {
		            ContentType = "application/json",
		            StatusCode = HttpStatusCode.OK,
		            Contents = (stream) => stream.Write(bytes, 0, bytes.Length)
	            };
            };
        }
    }
}
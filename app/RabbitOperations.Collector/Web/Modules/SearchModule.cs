using Nancy;

namespace RabbitOperations.Collector.Web.Modules
{
    public class SearchModule : NancyModule
    {
        public SearchModule() : base("/Search")
        {
            Get["/"] = parameters =>
            {

                return View["index.cshtml"];
            };
        }
    }
}
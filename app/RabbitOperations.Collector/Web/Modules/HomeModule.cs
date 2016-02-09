using Nancy;

namespace RabbitOperations.Collector.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule() : base("/Home")
        {
            Get["/"] = parameters =>
            {

                return View["index.cshtml"];
            };
        }
    }
}
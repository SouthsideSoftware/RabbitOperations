using Nancy;

namespace RabbitOperations.Collector.Web.Modules
{
    public class DashboardModule : NancyModule
    {
        public DashboardModule() 
        {
            Get["/"] = parameters =>
            {
                
                return View["search.cshtml"];
            };

            Get["/Search"] = parameters =>
            {

                return View["search.cshtml"];
            };

            Get["/Tail"] = parameters =>
            {

                return View["tail.cshtml"];
            };
        }

    }
}
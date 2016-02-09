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

                return View["Search/index.cshtml"];
            };

            Get["/Tail"] = parameters =>
            {

                return View["tail.cshtml"];
            };

            Get["/Home"] = parameters =>
            {

                return View["Home/Index.cshtml"];
            };
        }

    }
}
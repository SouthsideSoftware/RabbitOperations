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

            
        }

    }
}
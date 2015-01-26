using Nancy;

namespace RabbitOperations.Collector.Web.Modules.Hello
{
    public class DashboardModule : NancyModule
    {
        public DashboardModule() 
        {
            Get["/"] = parameters =>
            {
                
                return View["dashboard.cshtml"];
            };
        }
    }
}
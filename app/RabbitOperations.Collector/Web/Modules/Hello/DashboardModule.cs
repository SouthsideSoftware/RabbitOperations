using Microsoft.AspNet.SignalR;
using Nancy;
using RabbitOperations.Collector.Web.SignalRHubs;

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
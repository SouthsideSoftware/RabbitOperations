using Nancy;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class QueuePollersModule : NancyModule
    {
        private readonly IActiveApplicationListeners activeApplicationListeners;

        public QueuePollersModule(IActiveApplicationListeners activeApplicationListeners) : base("Api/V1/QueuePollers")
        {
            this.activeApplicationListeners = activeApplicationListeners;
            Verify.RequireNotNull(activeApplicationListeners, "activeQueuePollers");

            this.activeApplicationListeners = activeApplicationListeners;
            
            Get["/"] = parameters =>
            {

                return activeApplicationListeners;
            };
        }
    }
}
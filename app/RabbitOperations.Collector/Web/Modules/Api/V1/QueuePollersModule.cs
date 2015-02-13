using Nancy;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class QueuePollersModule : NancyModule
    {
        private readonly IActiveQueuePollers activeQueuePollers;

        public QueuePollersModule(IActiveQueuePollers activeQueuePollers) : base("Api/V1/QueuePollers")
        {
            this.activeQueuePollers = activeQueuePollers;
            Verify.RequireNotNull(activeQueuePollers, "activeQueuePollers");

            this.activeQueuePollers = activeQueuePollers;
            
            Get["/"] = parameters =>
            {

                return activeQueuePollers;
            };
        }
    }
}
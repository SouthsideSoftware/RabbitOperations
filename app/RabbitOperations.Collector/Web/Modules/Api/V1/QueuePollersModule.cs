using Nancy;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class QueuePollersModule : NancyModule
    {
        public QueuePollersModule() : base("Api/V1/QueuePollers")
        {
            Get["/"] = parameters =>
            {

                return "stuff for v1";
            };
        }

    }
}
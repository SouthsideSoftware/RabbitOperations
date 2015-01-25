using Nancy;

namespace RabbitOperations.Collector.Web
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Hello World";
        }
    }
}
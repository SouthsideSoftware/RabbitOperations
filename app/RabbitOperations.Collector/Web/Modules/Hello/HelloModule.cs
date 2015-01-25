using Nancy;

namespace RabbitOperations.Collector.Web.Modules.Hello
{
    public class HelloModule : NancyModule
    {
        public HelloModule() 
        {
            Get["/"] = parameters =>
            {
                
                return View["hello.cshtml"];
            };
        }
    }
}
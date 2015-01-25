using Owin;

namespace RabbitOperations.Collector.Web.Startup
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}
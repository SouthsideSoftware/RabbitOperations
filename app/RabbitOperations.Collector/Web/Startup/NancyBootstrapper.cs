using Castle.Windsor;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;
using Nancy.Conventions;
using SouthsideUtility.Core.CastleWindsor;
using System.Web.Routing;

namespace RabbitOperations.Collector.Web.Startup
{
    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }
        protected override IWindsorContainer GetApplicationContainer()
        {
            return ServiceLocator.Container;
        }
    }
}
using Castle.Windsor;
using Nancy;
using Nancy.Bootstrappers.Windsor;
using SouthsideUtility.Core.CastleWindsor;

namespace RabbitOperations.Collector.Web.Startup
{
    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
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
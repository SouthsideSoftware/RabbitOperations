using System;
using Castle.Windsor;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;
using Nancy.Conventions;
using SouthsideUtility.Core.CastleWindsor;
using System.Web.Routing;
using Metrics;
using Metrics.Reports;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using RabbitOperations.Collector.Web.SignalR;

namespace RabbitOperations.Collector.Web.Startup
{
    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            Metric.Config
                .WithAllCounters().WithReporting(x => x.WithReport(new SingalrMetricsReport("RabbitOperations"), TimeSpan.FromMilliseconds(2000)))
                .WithNancy(pipelines);
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
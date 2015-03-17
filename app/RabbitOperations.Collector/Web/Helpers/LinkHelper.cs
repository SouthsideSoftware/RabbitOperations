using Nancy.ViewEngines.Razor;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using Raven.Abstractions.Data;
using SouthsideUtility.Core.CastleWindsor;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.Helpers
{
    public static class LinkHelper
    {
        public static string RavenStudioUrl<T>(this HtmlHelpers<T> helpers, ISettings settings = null)
        {
            Verify.RequireNotNull(helpers, "helpers");

            if (settings == null)
            {
                settings = ServiceLocator.Container.Resolve<ISettings>();
            }
            Verify.RequireNotNull(settings, "settings");

            return RavenStudioUrl(settings, helpers.RenderContext.Context.Request.Url.HostName);
        }

        public static string RavenStudioUrl(ISettings settings, string hostName)
        {
            Verify.RequireStringNotNullOrWhitespace(hostName, "hostName");
            Verify.RequireNotNull(settings, "settings");

            if (settings.EmbedRavenDB)
            {
                return string.Format("http://{0}:{1}", hostName,
                    settings.EmbeddedRavenDBManagementPort);
            }
            else
            {
                var connectionStringParser =
                    ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(
                        settings.RavenDBConnectionString);
                connectionStringParser.Parse();
                return connectionStringParser.ConnectionStringOptions.Url;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.Extensions.OptionsModel;
using RabbitOperations.Collector.Configuration;
using RabbitOperations.Collector.Configuration.Interfaces;
using Raven.Abstractions.Data;
using SouthsideUtility.Core.DependencyInjection;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web
{
    public static class RavenLinkHelper
    {
        public static string RavenStudioUrl(IOptions<RavenDbSettings> settingsSection = null, IHttpContextAccessor contextAccessor = null)
        {
            if (settingsSection == null)
            {
                settingsSection = ServiceLocator.Container.Resolve<IOptions<RavenDbSettings>>();
            }
            if (contextAccessor == null)
            {
                contextAccessor = ServiceLocator.Container.Resolve<IHttpContextAccessor>();
            }

            var settings = settingsSection.Value;


            if (settings.UseEmbedded)
            {
                return string.Format("http://{0}:{1}", contextAccessor.HttpContext.Request.Host,
                    settings.EmbeddedRavenDb.ManagementPort);
            }

            return settings.ExternalRavenDb.Url;
        }
    }
}

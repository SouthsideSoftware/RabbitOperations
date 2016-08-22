using Nancy;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;
using System.Linq;
using Nancy.ModelBinding;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Web.Modules.Api.V1
{
    public class ApplicationsModule : NancyModule
    {

        ISettings settings;
        public ApplicationsModule(ISettings settings) : base("Api/V1/Applications")
        {
            Verify.RequireNotNull(settings, "settings");
            this.settings = settings;

            //gets a list of all applications
            Get["/{id?}"] = parameters =>
            {
                if (parameters.id == null)
                {
                    return settings.Applications;
                }
                else
                {
                    var application = settings.Applications.FirstOrDefault(x => x.ApplicationId == parameters.id);
                    if (application == null)
                    {
                        return HttpStatusCode.NotFound;
                    }
                    return application;
                }
            };

            //Add or updates an application
            Post["/{id}"] = parameters =>
            {
                var existingApplicaiton = settings.Applications.FirstOrDefault(x => x.ApplicationId == parameters.id);
                if (existingApplicaiton != null)
                {
                    settings.Applications.Remove(existingApplicaiton);
                }
                var application = this.Bind<ApplicationConfiguration>();
                settings.Applications.Add(application);
                return settings.Applications;
            };

            Delete["/{id}"] = parameters =>
            {
                var existingApplicaiton = settings.Applications.FirstOrDefault(x => x.ApplicationId == parameters.id);
                if (existingApplicaiton != null)
                {
                    settings.Applications.Remove(existingApplicaiton);
                }

                return settings.Applications;
            };


        }
    }
}
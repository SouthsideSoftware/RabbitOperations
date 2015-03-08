using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.SchemaUpdates
{
    public class ToVersion1 : IUpdateSchemaVersion
    {
        private readonly ISettings settings;
        public ToVersion1(ISettings settings)
        {
            Verify.RequireNotNull(settings, "settings");

            this.settings = settings;
        }

        public int SchemaVersion
        {
            get { return 1; }
        }

        public void UpdateSchema()
        {
            //In this case, all we have to do is save
            settings.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Owin.Security;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public class SchemaUpdater : ISchemaUpdater
    {
        private readonly IList<IUpdateSchema> schemaUpdaters;
        private readonly ISettings settings;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public SchemaUpdater(IList<IUpdateSchema> schemaUpdaters, ISettings settings)
        {
            Verify.RequireNotNull(schemaUpdaters, "schemaUpdaters");
            Verify.RequireNotNull(settings, "settings");

            this.schemaUpdaters = schemaUpdaters;
            this.settings = settings;

            schemaUpdaters = schemaUpdaters.Where(x => x.SchemaVersion > settings.DatabaseSchemaVersion).OrderBy(x => x.SchemaVersion).ToList();
        }

        public void UpdateSchema()
        {
            logger.Info("Current database schema is version {0}", settings.DatabaseSchemaVersion);
            if (schemaUpdaters.Count > 0)
            {
                logger.Info("Database will be upgraded to version {0}", schemaUpdaters.Last().SchemaVersion);
                foreach (var schemaUpdater in schemaUpdaters)
                {
                    logger.Info("Updating schema to version {0}", schemaUpdater.SchemaVersion);
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            schemaUpdater.UpdateSchema();
                            settings.DatabaseSchemaVersion = schemaUpdater.SchemaVersion;
                            settings.Save();
                            transaction.Complete();
                            logger.Info("Schema updated to version {0}", schemaUpdater.SchemaVersion);
                        }
                        catch (Exception err)
                        {
                            logger.Error(
                                "Schema upgrade to version {0} failed with error {1}.  All changed will rollback.",
                                schemaUpdater.SchemaVersion, err);
                            break;
                        }
                    }
                }
            }
        }
    }
}

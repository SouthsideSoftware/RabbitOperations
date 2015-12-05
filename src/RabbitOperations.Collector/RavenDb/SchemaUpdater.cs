using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Serilog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB
{
    public class SchemaUpdater : ISchemaUpdater
    {
        private readonly IQualifiedSchemaUpdatersFactory qualifiedSchemaUpdatersFactory;
        private readonly ISettings settings;


        public SchemaUpdater(IQualifiedSchemaUpdatersFactory qualifiedSchemaUpdatersFactory, ISettings settings)
        {
            Verify.RequireNotNull(settings, "settings");
            Verify.RequireNotNull(qualifiedSchemaUpdatersFactory, "qualifiedSchemaUpdatersFactory");

            this.qualifiedSchemaUpdatersFactory = qualifiedSchemaUpdatersFactory;
            this.settings = settings;
        }

        public void UpdateSchema()
        {
            Log.Logger.Information("Current database schema is version {SchemaVersion}", settings.DatabaseSchemaVersion);
            var schemaUpdaters = qualifiedSchemaUpdatersFactory.Get();
            if (schemaUpdaters.Count > 0)
            {
                Log.Logger.Information("Database will be upgraded to version {NewSchemaVersion}", schemaUpdaters.Last().SchemaVersion);
                foreach (var schemaUpdater in schemaUpdaters)
                {
                    Log.Logger.Information("Updating schema to version {NewSchemaVersion}", schemaUpdater.SchemaVersion);
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            schemaUpdater.UpdateSchema();
                            settings.DatabaseSchemaVersion = schemaUpdater.SchemaVersion;
                            settings.Save();
                            transaction.Complete();
                            Log.Logger.Information("Schema updated to version {NewSchemaVersion}", schemaUpdater.SchemaVersion);
                        }
                        catch (Exception err)
                        {
                            Log.Logger.Error(
                                "Schema upgrade to version {NewSchemaVersion} failed with error {Error}.  All changed will rollback.",
                                schemaUpdater.SchemaVersion, err);
                            break;
                        }
                    }
                }
            }
        }
    }
}

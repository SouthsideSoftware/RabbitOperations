using System.Collections.Generic;
using NLog;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.RavenDB.Indexes;
using RabbitOperations.Collector.RavenDB.Interfaces;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.RavenDB.SchemaUpdates
{
	public class ToVersion0023 : IUpdateSchemaVersion
	{
		private readonly ISettings settings;
		private readonly IDocumentStore store;
		public Logger logger = LogManager.GetCurrentClassLogger();

		public ToVersion0023(ISettings settings, IDocumentStore store)
		{
			Verify.RequireNotNull(settings, "settings");
			Verify.RequireNotNull(store, "store");

			this.settings = settings;
			this.store = store;
		}

		public int SchemaVersion
		{
			get { return 23; }
		}

		public void UpdateSchema()
		{
			store.ExecuteUpdateByIndexOnDefaultTenant("Raven/DocumentsByEntityName",
				new IndexQuery(),
				new ScriptedPatchRequest()
				{
					Script = "this.DocId = Number(__document_id.split('/')[1])"
				});
			logger.Info("Updating MessageDocumentSearch index");
			store.ExecuteIndexCreationOnDefaultTenant(new MessageDocument_Search());
		}
	}
}
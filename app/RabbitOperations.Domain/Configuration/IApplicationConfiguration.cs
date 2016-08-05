namespace RabbitOperations.Domain.Configuration
{
	public interface IApplicationConfiguration
	{
		string ApplicationId { get; set; }
		string ApplicationName { get; set; }
		string AuditQueue { get; set; }
		string ErrorQueue { get; set; }
		string RabbitConnectionString { get; set; }

		/// <summary>
		/// The maximum number of messages that will be read in one run of the application.
		/// This should only be set for troubleshooting or configuration purposes.
		/// Normally, this is set to zero, which indicates no limit
		/// </summary>
		int MaxMessagesPerRun { get; set; }

		int PollingTimeoutMilliseconds { get; set; }

		/// <summary>
		/// The app keeps the connection alive by sending peiodic heartbeats.
		/// This should be set below 20 seconds when running behind an ELB because
		/// they use a relatively short timeout.
		/// </summary>
		int HeartbeatIntervalSeconds { get; set; }

		bool AutoStartQueuePolling { get; set; }
		int DocumentExpirationInHours { get; set; }
		int ErrorDocumentExpirationInHours { get; set; }
		int RabbitManagementPort { get; set; }
		ushort Prefetch { get; set; }
	}
}
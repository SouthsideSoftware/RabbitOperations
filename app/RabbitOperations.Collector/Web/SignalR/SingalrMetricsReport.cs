using System;
using System.Linq;
using System.Threading;
using Metrics;
using Metrics.MetricData;
using Metrics.Reporters;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using NLog;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.SignalR
{
    public class SingalrMetricsReport : MetricsReport
    {
        private readonly string counterType;
        private readonly string prefix;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private IHubConnectionContext<dynamic> clients; 

        public SingalrMetricsReport(string counterType)
        {
            Verify.RequireStringNotNullOrWhitespace(counterType, "counterType");

            this.counterType = counterType;
            this.prefix = string.Format("{0}.", this.counterType);

            clients = GlobalHost.ConnectionManager.GetHubContext<MessagePulseHub>().Clients;
        }

        public void RunReport(MetricsData metricsData, Func<HealthStatus> healthStatus, CancellationToken token)
        {
            var data = JsonConvert.SerializeObject(metricsData.Meters.Where(x => x.Name.StartsWith(prefix)).ToList());
            logger.Trace($"Sending message of length {data.Length} to clients. Message: {data}");

            clients.All.pulse("Metrics", data);
        }
    }
}
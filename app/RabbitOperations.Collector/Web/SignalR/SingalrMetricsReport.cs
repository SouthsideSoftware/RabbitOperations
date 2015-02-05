using System;
using System.Linq;
using System.Threading;
using Metrics;
using Metrics.MetricData;
using Metrics.Reporters;
using Newtonsoft.Json;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Web.SignalR
{
    public class SingalrMetricsReport : MetricsReport
    {
        private readonly string counterType;
        private readonly string prefix;

        public SingalrMetricsReport(string counterType)
        {
            Verify.RequireStringNotNullOrWhitespace(counterType, "counterType");

            this.counterType = counterType;
            this.prefix = string.Format("{0}.", this.counterType);
        }

        public void RunReport(MetricsData metricsData, Func<HealthStatus> healthStatus, CancellationToken token)
        {
            MessagePulseHub.SendMessage("Metrics", JsonConvert.SerializeObject(metricsData.Meters.Where(x => x.Name.StartsWith(prefix)).ToList()));
        }
    }
}
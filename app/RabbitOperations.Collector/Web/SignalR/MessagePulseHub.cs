using Microsoft.AspNet.SignalR;

namespace RabbitOperations.Collector.Web.SignalR
{
    public class MessagePulseHub : Hub
    {
        public string Activate()
        {
            return "activated";
        }
    }
}
using Microsoft.AspNet.SignalR;

namespace RabbitOperations.Collector.Web.SignalR
{
    public class MessagePulseHub : Hub
    {
        public string Activate()
        {
            return "activated";
        }

        public static void SendMessage(string messageType, string message)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<MessagePulseHub>().Clients.All.pulse(messageType, message);
        }
    }
}

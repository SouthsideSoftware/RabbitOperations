using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace RabbitOperations.Collector.Web.SignalRHubs
{
    public class MessagePulseHub : Hub
    {
        public string Activate()
        {
            return "activated";
        }

        public static void SendMessage(string message)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<MessagePulseHub>().Clients.All.pulse(message);
        }
    }
}

using System;
using System.CodeDom;
using System.Runtime.CompilerServices;
using PowerArgs;

namespace SendTestMessages.CommandLine
{
    public class Arguments
    {
        [HelpHook]
        [ArgShortcut("-?")]
        [ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-s")]
        [ArgDescription("The RabbitMQ server")]
        public string RabbitServer { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-v")]
        [ArgDescription("The RabbitMQ VHost")]
        public string RabbitVHost { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-u")]
        [ArgDescription("The RabbitMQ user")]
        public string RabbitUserName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-p")]
        [ArgDescription("The RabbitMQ password")]
        public string RabbitPassword { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-q")]
        [ArgDescription("The name of the queue to receive messages")]
        public string QueueName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-m")]
        [ArgDefaultValue(60)]
        [ArgRange(0, int.MaxValue)]
        [ArgDescription("The number of dummy messages to send per minute")]
        public int MessagesPerMinute { get; set; }

        public void Main()
        {
            if (string.IsNullOrWhiteSpace(RabbitServer))
            {
                throw new Exception("RabbitServer (-s) must not be null or whitespace");
            }
            if (string.IsNullOrWhiteSpace(QueueName))
            {
                
            }
            var credentials = !string.IsNullOrWhiteSpace(RabbitUserName) && !string.IsNullOrWhiteSpace(RabbitPassword)
                ? string.Format("{0}:{1}@", RabbitUserName, RabbitPassword) : "";
            var vhost = !string.IsNullOrWhiteSpace(RabbitVHost) ? String.Format("/{0}", RabbitVHost) : "";
            var connectionString = string.Format("amqp://{0}{1}{2}", credentials, RabbitServer, vhost);
            Console.WriteLine("Connection String: {0}", connectionString);
            Console.WriteLine("QueueName: {0}", QueueName);
            Console.WriteLine("Messages per Minute: {0}", MessagesPerMinute);
        }
    }
}

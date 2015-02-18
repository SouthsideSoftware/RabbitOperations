using System;
using System.CodeDom;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArgs;
using RabbitMQ.Client;

namespace SendTestMessages.CommandLine
{
    public class CommandLineApplication
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
        [ArgShortcut("-e")]
        [ArgDescription("The name of the exchange to receive messages")]
        public string Exchange { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-m")]
        [ArgDefaultValue(60)]
        [ArgRange(0, int.MaxValue)]
        [ArgDescription("The number of dummy messages to send per minute")]
        public int MessagesPerMinute { get; set; }

        private CancellationToken cancellationToken = new CancellationToken();
        private Task senderTask;
        private string connectionString;

        public void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            connectionString = GetRabbitConnectionString();
            WriteParametersToConsole(connectionString);

            SendUntilStopped();
            Console.WriteLine("Sending.  Press CTRL-C to stop");
            Console.CancelKeyPress += (sender, args) =>
            {
                cancellationTokenSource.Cancel();
                Console.WriteLine("Stopping...");
                Task.WaitAll(new[] {senderTask});
                Console.WriteLine("Done sending");
            };
            Console.ReadKey();
        }

        private void SendUntilStopped()
        {
            senderTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    var factory = new ConnectionFactory() { Uri = connectionString };
                    using (var connection = factory.CreateConnection())
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            using (var channel = connection.CreateModel())
                            {
                                string message = "Hello World!";
                                var body = Encoding.UTF8.GetBytes(message);

                                channel.BasicPublish(Exchange, "", null, body);
                                Console.Write(".");
                            }
                            Thread.Sleep(60000 / MessagesPerMinute);
                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("Failed with error {1}", err);
                    throw;
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void WriteParametersToConsole(string connectionString)
        {
            Console.WriteLine("Connection String (-s, -v, -u, -p): {0}", connectionString);
            Console.WriteLine("Exchange (-e): {0}", Exchange);
            Console.WriteLine("Messages per Minute (-m): {0}", MessagesPerMinute);
        }

        private string GetRabbitConnectionString()
        {
            if (string.IsNullOrWhiteSpace(RabbitServer))
            {
                throw new Exception("RabbitServer (-s) must not be null or whitespace");
            }
            if (string.IsNullOrWhiteSpace(Exchange))
            {
                throw new Exception("Exchange (-e) must not be null or whitespace");
            }
            var credentials = !string.IsNullOrWhiteSpace(RabbitUserName) && !string.IsNullOrWhiteSpace(RabbitPassword)
                ? string.Format("{0}:{1}@", RabbitUserName, RabbitPassword)
                : "";
            var vhost = !string.IsNullOrWhiteSpace(RabbitVHost) ? String.Format("/{0}", RabbitVHost) : "";
            var connectionString = string.Format("amqp://{0}{1}{2}", credentials, RabbitServer, vhost);
            return connectionString;
        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using PowerArgs;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.Tests.Unit;

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
        [ArgDescription("The RabbitMQ server. Blank for localhost.")]
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
        [ArgShortcut("-t")]
        [ArgDescription("The name of the message (error, audit or emptybody).  Blank for dummy message.")]
        public string Template { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-m")]
        [ArgRange(0, int.MaxValue)]
        [ArgDescription("The number of dummy messages to send per minute")]
        public int MessagesPerMinute { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("-mx")]
        [ArgRange(0, int.MaxValue)]
        [ArgDescription("The maximum number of messages to send (0 for infinite)")]
        public int MaxMessages { get; set; }

        private CancellationToken cancellationToken = new CancellationToken();
        private Task senderTask;
        private string connectionString;
        private int consecutiveRetryCount;

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
                var retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetry(120, retryCount => GetRetryDelay(), (exception, retryDelay, context) =>
                    {
                        Console.WriteLine($"Retry after {retryDelay}");
                    });
                retryPolicy.Execute(SendMessages);
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Exponetial backoff of timeout until we hit the sixth retry.
        /// After that, its 2^5 (32) seconds per retry
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetRetryDelay()
        {
            consecutiveRetryCount++;
            if (consecutiveRetryCount <= 5)
            {
                return TimeSpan.FromSeconds(Math.Pow(2, consecutiveRetryCount));
            }
            return TimeSpan.FromSeconds(Math.Pow(2, 5));
        }

        private void SendMessages()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested) return;
                byte[] body = null;
                Dictionary<string, object> headers = null;
                if (!string.IsNullOrWhiteSpace(Template))
                {
                    try
                    {
                        var rawMessage = MessageTestHelpers.GetMessageFromFile(Template);
                        var data = rawMessage.GetEelementsForRabbitPublish();
                        body = data.Item1;
                        headers = data.Item2;
                        Console.WriteLine($"Sending message from {Template}");
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"Could not get message from {Template}.  The error is {err}");
                        throw;
                    }
                }

                if (string.IsNullOrWhiteSpace(Template) || (body == null && headers == null))
                {
                    body = Encoding.UTF8.GetBytes("Hello World!");
                    headers = new Dictionary<string, object>
                    {
                        {"one", Encoding.UTF8.GetBytes("Header value")}
                    };
                    Console.WriteLine("Sending HelloWorld!");
                }

                var factory = new ConnectionFactory() {Uri = connectionString};
                using (var connection = factory.CreateConnection())
                {
                    long messageCount = 0;
                    while (!cancellationToken.IsCancellationRequested && (MaxMessages == 0 || messageCount < MaxMessages))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.BasicPublish(Exchange, "", new BasicProperties {Headers = headers}, body);
                            messageCount++;
                            Console.Write(".");
                        }
                        Thread.Sleep(60000/MessagesPerMinute);
                        consecutiveRetryCount = 0;
                    }

                    Console.WriteLine("Done");
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed with error {err}");
                throw;
            }
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
                RabbitServer = "localhost";
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

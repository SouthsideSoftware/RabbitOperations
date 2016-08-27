using NLog;
using RabbitMQ.Client;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;
using System;
using System.Linq;

namespace RabbitOperations.Collector.MessageRetry
{
    public class SendMessagesService : ISendMessages
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IRabbitConnectionFactory rabbitConnectionFactory;
        private readonly ISettings settings;

        public SendMessagesService(IRabbitConnectionFactory rabbitConnectionFactory, ISettings settings)
        {
            Verify.RequireNotNull(rabbitConnectionFactory, "rabbitConnectionFactory");
            Verify.RequireNotNull(settings, "settings");

            this.rabbitConnectionFactory = rabbitConnectionFactory;
            this.settings = settings;
        }

        public string Send(IRawMessage message, string queueName, bool replayToExchange, string applicationId, IBasicProperties basicProperties)
        {
            Verify.RequireNotNull(message, "message");
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireStringNotNullOrWhitespace(applicationId, "applicationId");

            try
            {
                var application = settings.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);
                if (application == null)
                {
                    return $"Could not find configuration for application id {applicationId}";
                }

                using (
                    var connection =
                        rabbitConnectionFactory.Create(application.RabbitConnectionString).CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        if (replayToExchange)
                        {
                            channel.ExchangeDeclarePassive(queueName);
                        }
                        else
                        {
                            channel.QueueDeclarePassive(queueName);
                        }
                        var sendData = message.GetEelementsForRabbitPublish();
                        basicProperties.Headers = sendData.Item2;
                        if (replayToExchange)
                        {
                            channel.BasicPublish(queueName, string.Empty, basicProperties, sendData.Item1);
                        }
                        else
                        {
                            channel.BasicPublish(string.Empty, queueName, basicProperties, sendData.Item1);
                        }
                        return null;
                    }
                }
            }
            catch (Exception err)
            {
                logger.Error(err, "Failed to send message for retry");
                return $"Failed to send message for retry.  Error is {err.Message}.  See error log for details";
            }
        }
    }
}
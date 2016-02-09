using System;
using System.Linq;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser.Interfaces;
using RabbitOperations.Collector.MessageRetry.Interfaces;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain;
using SouthsideUtility.Core.DesignByContract;

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

        /// <summary>
        ///     Sends a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <param name="applicationId"></param>
        /// <param name="basicProperties"></param>
        /// <returns>Null on success or the text of an error message</returns>
        public string Send(IRawMessage message, string queueName, string applicationId, IBasicProperties basicProperties)
        {
            Verify.RequireNotNull(message, "message");
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireStringNotNullOrWhitespace(applicationId, "applicationId");

            try
            {
                var application = settings.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);
                if (application == null)
                {
                    return string.Format("Could not find configuration for application id {0}", applicationId);
                }

                using (
                    var connection =
                        rabbitConnectionFactory.Create(application.RabbitConnectionString).CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var sendData = message.GetEelementsForRabbitPublish();
                        basicProperties.Headers = sendData.Item2;
                        channel.BasicPublish(queueName, "", basicProperties, sendData.Item1);
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
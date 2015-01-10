﻿using System.Collections.Generic;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Configuration.Interfaces
{
    public interface ISettings
    {
        string AuditQueue { get; set; }
        string ErrorQueue { get; set; }

        IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }

        void Load();
        void Save();
    }
}

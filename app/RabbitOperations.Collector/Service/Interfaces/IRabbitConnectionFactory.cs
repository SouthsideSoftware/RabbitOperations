﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IRabbitConnectionFactory
    {
        IConnectionFactory Create(string connectionString, ushort heartbeatIntervalSeconds = 10);
    }
}

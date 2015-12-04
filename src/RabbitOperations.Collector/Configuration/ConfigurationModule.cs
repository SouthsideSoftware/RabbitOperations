using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using RabbitOperations.Collector.Configuration.Interfaces;

namespace RabbitOperations.Collector.Configuration
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Settings>().As<ISettings>().SingleInstance();
        }
    }
}

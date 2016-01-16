using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using RabbitOperations.Collector.RavenDb.Query.Interfaces;

namespace RabbitOperations.Collector.RavenDb.Query
{
    public class QueryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BasicSearch>().As<IBasicSearch>().SingleInstance();
        }
    }
}

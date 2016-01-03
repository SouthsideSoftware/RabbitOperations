using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Serilog;
using Xunit.Abstractions;

namespace RabbitOperations.Collector.Tests.Unit
{
    public class Test1 : TestIncludesLogging
    {
        public Test1(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            
        }

        [Fact]
        public void Pass()
        {
            OutputHelper.WriteLine("Hello");
            Log.Information("Pass");
            int x = 1;
            int y = 2;
            int z = x + y;
            z.Should().Be(3);
        }

        [Fact]
        public void Fail()
        {
            Log.Information("fail");
            int x = 1;
            int y = 2;
            int z = x + y;
            z.Should().Be(4);
        }
    }
}

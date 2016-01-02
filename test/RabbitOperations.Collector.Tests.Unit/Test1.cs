using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace RabbitOperations.Collector.Tests.Unit
{
    public class Test1
    {
        [Fact]
        public void Pass()
        {
            int x = 1;
            int y = 2;
            int z = x + y;
            z.Should().Be(3);
        }

        [Fact]
        public void Fail()
        {
            int x = 1;
            int y = 2;
            int z = x + y;
            z.Should().Be(4);
        }
    }
}

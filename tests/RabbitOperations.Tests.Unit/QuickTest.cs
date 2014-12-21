using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace RabbitOperations.Tests.Unit
{
    [TestFixture]
    public class QuickTest
    {
        [Test]
        public void Test1()
        {
            int x = 1;
            int y = 1;
            int z = x + y;
            z.Should().Be(2);
        }
    }
}

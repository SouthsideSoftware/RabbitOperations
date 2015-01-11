using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Domain;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    [TestFixture]
    public class TypeNameTests
    {
        [Test]
        public void CanConstructFromFullName()
        {
            //arrange and act
            var typeName =
                new TypeName(
                    "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            //assert
            typeName.Should().Be(new TypeName
            {
                Assembly = "Autobahn.Fulfillment.Contracts",
                ClassName = "NotifyOrderHasBeenCanceled",
                Culture = "neutral",
                Namespace = "Autobahn.Fulfillment.Contracts.Ordering",
                PublicKeyToken = "null",
                Version = new Version(1, 0, 0, 0)
            });
        }

        [Test]
        public void CanConstructFromFullNameWhenClassNameHasNoNamespace()
        {
            //arrange and act
            var typeName =
                new TypeName(
                    "NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            //assert
            typeName.Should().Be(new TypeName
            {
                Assembly = "Autobahn.Fulfillment.Contracts",
                ClassName = "NotifyOrderHasBeenCanceled",
                Culture = "neutral",
                Namespace = "",
                PublicKeyToken = "null",
                Version = new Version(1, 0, 0, 0)
            });
        }

        [Test]
        public void ShouldReturnAFullClassStringFromToString()
        {
            //arrange and act
            var fullName = "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var typeName =
                new TypeName(
                    fullName);

            //assert
            typeName.ToString().Should().Be(fullName);
        }
       
    }
}

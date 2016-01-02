using System;
using FluentAssertions;
using RabbitOperations.Domain;
using Xunit;

namespace RabbitOperations.Collector.Tests.Unit.MessageParser
{
    public class TypeNameTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
        public void ShouldReturnAFullClassStringFromToString()
        {
            //arrange and act
            var fullName =
                "Autobahn.Fulfillment.Contracts.Ordering.NotifyOrderHasBeenCanceled, Autobahn.Fulfillment.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var typeName =
                new TypeName(
                    fullName);

            //assert
            typeName.ToString().Should().Be(fullName);
        }
    }
}
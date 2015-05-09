using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using RabbitOperations.Collector.Poller;

//namespace RabbitOperations.Collector.Tests.Unit.Poller
//{
//    [TestFixture]
//    public class PollerTests
//    {
//        [Test]
//        public void StartShouldChangeStatusToPolling()
//        {
//            //arrange
//            var poller = new SimpleTestPollingHost();

//            //act
//            poller.Start();

//            //assert
//            poller.Status.Should().Be(PollerStatus.Polling);
//        }

//        [Test]
//        public void StopAfetrStartShouldChangeStatusToStopped()
//        {
//            //arrange
//            var poller = new SimpleTestPollingHost();
//            poller.Start();

//            //act
//            poller.Stop();

//            //assert
//            poller.Status.Should().Be(PollerStatus.Stopped);
//        }

//        [Test]
//        public void PollerShouldBeginWithStoppedStatus()
//        {
//            //arrange and act
//            var poller = new SimpleTestPollingHost();

//            //assert
//            poller.Status.Should().Be(PollerStatus.Stopped);
//        }

//        public class SimpleTestPollingHost : PollerHost
//        {
//            public override void Poll()
//            {
//                //do work
//            }

//            public override string Name
//            {
//                get { return "SimpleTestPoller1"; }
//            }

//            public SimpleTestPollingHost(CancellationToken cancellationToken) : base(cancellationToken)
//            {
//            }
//        }
//    }
//}

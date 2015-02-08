using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Moq.AutoMock;
using FluentAssertions;
using RabbitOperations.Collector.Configuration.Interfaces;
using RabbitOperations.Collector.MessageParser;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    [TestFixture]
    public class ActiveQueuePollersTests
    {
        [Test]
        public void ShouldBeAbleToAdd()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1);

            //act
            activeQueuePollers.Add(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.First().Key.Should().Be(key1);
        }

        [Test]
        public void ShouldBeAbleToRemove()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1);
            activeQueuePollers.Add(queuePoller);

            //act
            activeQueuePollers.Remove(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.Count.Should().Be(0);
        }

        [Test]
        public void ShouldNotDoAnythingWhenTryingToRemoveAPollerThatDoesNotExist()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1);
            activeQueuePollers.Add(queuePoller);

            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2);


            //act
            activeQueuePollers.Remove(queuePoller2);

            //assert 
            activeQueuePollers.ActivePollers.Should().BeEquivalentTo(new List<IQueuePoller> {queuePoller});
        }

        [Test]
        public void ShouldNotDoAnythingWhenAddingADuplicate()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1);

            activeQueuePollers.Add(queuePoller);

            //act
            activeQueuePollers.Add(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.First().Key.Should().Be(key1);
        }

        [Test]
        public void ShouldBeAbleToAddMultiples()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller1 = new QueuePoller(key1);
            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2);

            //act
            activeQueuePollers.Add(queuePoller1);
            activeQueuePollers.Add(queuePoller2);

            //assert 
            activeQueuePollers.ActivePollers.Select(p => p.Key).Should().BeEquivalentTo(new List<Guid> {key1, key2});
        }

        [Test]
        public void ShouldBeAbleToRemoveOneFromListOfMany()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller1 = new QueuePoller(key1);
            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2);

            activeQueuePollers.Add(queuePoller1);
            activeQueuePollers.Add(queuePoller2);

            //act
            activeQueuePollers.Remove(queuePoller1);

            //assert 
            activeQueuePollers.ActivePollers.Select(p => p.Key).Should().BeEquivalentTo(new List<Guid> { key2 });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using RabbitOperations.Collector.Service;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain.Configuration;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    public class ActiveQueuePollersTests
    {
        [Fact]
        public void ShouldBeAbleToAdd()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));

            //act
            activeQueuePollers.Add(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.First().Key.Should().Be(key1);
        }

        [Fact]
        public void ShouldBeAbleToRemove()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));
            activeQueuePollers.Add(queuePoller);

            //act
            activeQueuePollers.Remove(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldNotDoAnythingWhenTryingToRemoveAPollerThatDoesNotExist()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));
            activeQueuePollers.Add(queuePoller);

            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));


            //act
            activeQueuePollers.Remove(queuePoller2);

            //assert 
            activeQueuePollers.ActivePollers.Should().BeEquivalentTo(new List<IQueuePoller> {queuePoller});
        }

        [Fact]
        public void ShouldNotDoAnythingWhenAddingADuplicate()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));

            activeQueuePollers.Add(queuePoller);

            //act
            activeQueuePollers.Add(queuePoller);

            //assert 
            activeQueuePollers.ActivePollers.First().Key.Should().Be(key1);
        }

        [Fact]
        public void ShouldBeAbleToAddMultiples()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller1 = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));
            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));

            //act
            activeQueuePollers.Add(queuePoller1);
            activeQueuePollers.Add(queuePoller2);

            //assert 
            activeQueuePollers.ActivePollers.Select(p => p.Key).Should().BeEquivalentTo(new List<Guid> {key1, key2});
        }

        [Fact]
        public void ShouldBeAbleToRemoveOneFromListOfMany()
        {
            //arrange
            var activeQueuePollers = new ActiveQueuePollers();

            var key1 = Guid.NewGuid();
            var queuePoller1 = new QueuePoller(key1, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));
            var key2 = Guid.NewGuid();
            var queuePoller2 = new QueuePoller(key2, new QueueSettings("audit", new ApplicationConfiguration
            {
                ApplicationId = "test"
            }));

            activeQueuePollers.Add(queuePoller1);
            activeQueuePollers.Add(queuePoller2);

            //act
            activeQueuePollers.Remove(queuePoller1);

            //assert 
            activeQueuePollers.ActivePollers.Select(p => p.Key).Should().BeEquivalentTo(new List<Guid> {key2});
        }
    }
}
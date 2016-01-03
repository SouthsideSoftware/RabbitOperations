using FluentAssertions;
using RabbitOperations.Collector.MessageBusTechnologies.Common;
using RabbitOperations.Collector.Service;

namespace RabbitOperations.Collector.Tests.Unit.Service
{
    public class StoreMessageFactoryTests
    {
        //todo: Bring back this test
        //[OneTimeSetUp]
        //public void Setup()
        //{
        //    ServiceLocator.Container.Install(new Installer());
        //}

        //[OneTimeTearDown]
        //public void TearDown()
        //{
        //    ServiceLocator.ClearContainer();
        //}

        //[Test]
        //public void ShouldCreateMessageStoreServiceForRetry()
        //{
        //    //arrange
        //    var factory = new StoreMessagesFactory();

        //    var rawMessage = MessageTestHelpers.GetErrorMessage();
        //    rawMessage.Headers.Add(Headers.Retry, "test");

        //    //act
        //    var service = factory.MessageStorageServiceFor(rawMessage);

        //    //assert
        //    service.Should().BeOfType<StoreMessagesThatAreRetriesService>();
        //}

        //[Test]
        //public void ShouldCreateMessageStoreServiceForNonRetry()
        //{
        //    //arrange
        //    var factory = new StoreMessagesFactory();

        //    var rawMessage = MessageTestHelpers.GetErrorMessage();

        //    //act
        //    var service = factory.MessageStorageServiceFor(rawMessage);

        //    //assert
        //    service.Should().BeOfType<StoreMessagesThatAreNotRetriesService>();
        //}
    }
}
namespace KafkaFlow.IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.Consumers;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GlobalEventsTest
    {
        private readonly Fixture fixture = new();

        [TestMethod]
        public async Task SubscribeGlobalEvents_All_SubscribeCorrectly()
        {
            // Arrange
            bool isMessageProducedStarted = false, isMessageConsumeStarted = false, isMessageConsumeCompleted = false;

            Bootstrapper.GlobalEvents = observers =>
            {
                observers.MessageProduceStarted.Subscribe(eventContext =>
                {
                    isMessageProducedStarted = true;
                    return Task.CompletedTask;
                });

                observers.MessageConsumeStarted.Subscribe(eventContext =>
                {
                    isMessageConsumeStarted = true;
                    return Task.CompletedTask;
                });

                observers.MessageConsumeCompleted.Subscribe(eventContext =>
                {
                    isMessageConsumeCompleted = true;
                    return Task.CompletedTask;
                });
            };

            var provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();

            var consumer = provider.GetRequiredService<IConsumerAccessor>().All.First();
            var producer = provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
            var message = this.fixture.Create<TestMessage1>();

            // Act
            producer.Produce(message.Id.ToString(), message);

            await MessageStorage.AssertMessageAsync(message);

            // Assert
            Assert.IsTrue(isMessageProducedStarted);
            Assert.IsTrue(isMessageConsumeStarted);
            Assert.IsTrue(isMessageConsumeCompleted);
        }

        [TestMethod]
        public void SubscribeGlobalEvents_MessageContext_IsCorrect()
        {
            // Arrange
            IMessageContext messageContext = null;

            Bootstrapper.GlobalEvents = observers =>
            {
                observers.MessageProduceStarted.Subscribe(eventContext =>
                {
                    messageContext = eventContext.MessageContext;
                    return Task.CompletedTask;
                });
            };

            var provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();

            var consumer = provider.GetRequiredService<IConsumerAccessor>().All.First();
            var producer = provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
            var message = this.fixture.Create<TestMessage1>();

            // Act
            producer.Produce(message.Id.ToString(), message);

            // Assert
            Assert.IsNotNull(messageContext);
            Assert.AreEqual(messageContext.Message.Key, message.Id.ToString());
        }
    }
}

namespace KafkaFlow.IntegrationTests
{
    using System;
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
    public class ProducerTest
    {
        private readonly Fixture fixture = new();

        private IServiceProvider provider;

        private IMessageContext messageContext;

        [TestInitialize]
        public void Setup()
        {
            Bootstrapper.GlobalEvents = observers =>
            {
                observers.MessageProduceStarted.Subscribe(eventContext =>
                {
                    this.messageContext = eventContext.MessageContext;
                    return Task.CompletedTask;
                });
            };
            this.provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();
        }

        [TestMethod]
        public async Task ProduceNullKeyTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<GzipProducer>>();
            var message = this.fixture.Create<byte[]>();

            // Act
            await producer.ProduceAsync(null, message);

            // Assert
            await MessageStorage.AssertMessageAsync(message);
        }

        [TestMethod]
        public void SubscribeGlobalEventsTest()
        {
            var consumer = this.provider.GetRequiredService<IConsumerAccessor>().All.First();
            var producer = this.provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
            var messages = this.fixture.CreateMany<TestMessage1>(1).ToList();

            // Act
            messages.ForEach(m => producer.Produce(m.Id.ToString(), m));

            var x = messages.FirstOrDefault();

            // Assert
            Assert.IsNotNull(this.messageContext);
            Assert.AreEqual(this.messageContext.Message.Key, messages.FirstOrDefault().Id.ToString());
        }
    }
}

namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common.Core.Handlers;
    using KafkaFlow.IntegrationTests.Common.Core.Messages;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;

    [TestClass]
    public class BaseConsumerTest
    {
        private readonly Fixture fixture = new Fixture();

        public void Setup()
        {
            MessageStorage.Clear();
        }

        public async Task MultipleMessagesMultipleHandlersSingleTopicTest(IMessageProducer<JsonProducer> producer)
        {
            // Arrange
            var messages1 = this.fixture.CreateMany<TestMessage1>(5).ToList();
            var messages2 = this.fixture.CreateMany<TestMessage2>(5).ToList();
            
            // Act
            await Task.WhenAll(messages1.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));
            await Task.WhenAll(messages2.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            messages1.ForEach(async m => await MessageStorage.AssertMessageEqualAsync(m));
            messages2.ForEach(async m => await MessageStorage.AssertMessageEqualAsync(m));
        }

        public async Task MultipleTopicsSingleConsumerTest(
            IMessageProducer<ProtobufGzipProducer> producer1,
            IMessageProducer<ProtobufGzipProducer2> producer2)
        {
            // Arrange
            var messages = this.fixture.CreateMany<TestMessage1>(1).ToList();

            // Act
            messages.ForEach(m => producer1.Produce(m.Id.ToString(), m));
            messages.ForEach(m => producer2.Produce(m.Id.ToString(), m));

            // Assert
            messages.ForEach(async m => await MessageStorage.AssertCountMessagesAsync(m, 2));
        }

        public async Task MessageOrderingTest(IMessageProducer<ProtobufProducer> producer)
        {
            // Arrange
            var version = 1;
            var id = Guid.NewGuid();
            var messages = this.fixture
                .Build<TestMessage1>()
                .With(t => t.Id, id)
                .Without(t => t.Version)
                .Do(t => t.Version = version++)
                .CreateMany(5)
                .ToList();

            // Act
            messages.ForEach(async m => await producer.ProduceAsync(id.ToString(), m));

            // Assert
            await Task.Delay(1000);
            var versionsSent = messages.Select(m => m.Version).ToList();
            var versionsReceived = MessageStorage.GetVersionsFromMessage(id).ToList();

            //CollectionAssert.AreEqual(versionsSent, versionsReceived);
            CollectionAssert.AreEqual(versionsSent, versionsReceived, $"sent: {string.Join(",", versionsSent)} | received: {string.Join(", ", versionsReceived)} | ");
        }
    }
}

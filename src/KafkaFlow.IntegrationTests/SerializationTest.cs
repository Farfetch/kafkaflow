namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Producers;
    using KafkaFlow.Producers;
    using MessageTypes;

    [TestClass]
    internal class SerializationTest
    {
        private readonly Fixture fixture = new();

        private IServiceProvider provider;

        [TestInitialize]
        public void Setup()
        {
            this.provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();
        }

        [TestMethod]
        public async Task JsonMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<JsonProducer>>();
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task ProtobufMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<ProtobufProducer>>();
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task AvroMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<AvroProducer>>();
            var messages = this.fixture.CreateMany<LogMessages2>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task ProtobufSchemaRegistryMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<ConfluentProtobufProducer>>();
            var messages = this.fixture.CreateMany<TestProtoMessage>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id, m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task JsonSchemaRegistryMessageTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<ConfluentJsonProducer>>();
            var messages = this.fixture.CreateMany<TestMessage3>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }
    }
}

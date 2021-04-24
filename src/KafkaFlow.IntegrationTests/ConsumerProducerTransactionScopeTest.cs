namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Producers;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConsumerProducerTransactionScopeTest
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
        public async Task StreamingMessageHandlerTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<JsonInboundProducer>>();
            var messages = this.fixture.CreateMany<TestMessage3>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }

            // Wait for final commit
            Thread.Sleep(3000);
        }
    }
}

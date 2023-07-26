namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Producers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProducerTest
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
    }
}

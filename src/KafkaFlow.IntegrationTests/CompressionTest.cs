namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Handlers;
    using Core.Middlewares.Producers;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.Producers;

    [TestClass]
    public class CompressionTest
    {
        private IServiceProvider provider;
        private readonly Fixture fixture = new Fixture();

        [TestInitialize]
        public void Setup()
        {
            this.provider = Bootstrapper.GetServiceProvider();
            MessageStorage.Clear();
        }

        [TestMethod]
        public async Task GzipTest()
        {
            // Arrange
            var producer = this.provider.GetRequiredService<IMessageProducer<GzipProducer>>();
            var messages = this.fixture.CreateMany<byte[]>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));
            

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }
    }
}

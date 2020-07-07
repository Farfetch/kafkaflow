namespace KafkaFlow.IntegrationTests.Common
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.IntegrationTests.Common.Core.Handlers;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;

    public abstract class BaseCompressionTests
    {
        private readonly Fixture fixture = new Fixture();

        protected void Setup()
        {
            MessageStorage.Clear();
        }

        public async Task GzipTest(IMessageProducer<GzipProducer> producer)
        {
            // Arrange
            var messages = this.fixture.CreateMany<byte[]>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));

            // Assert
            messages.ForEach(async m => await MessageStorage.AssertMessageAsync(m));
        }
    }
}

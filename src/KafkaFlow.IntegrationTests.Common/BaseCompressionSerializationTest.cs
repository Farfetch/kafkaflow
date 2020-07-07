namespace KafkaFlow.IntegrationTests.Common
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.IntegrationTests.Common.Core.Handlers;
    using KafkaFlow.IntegrationTests.Common.Core.Messages;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;

    public abstract class BaseCompressionSerializationTest
    {
        private readonly Fixture fixture = new Fixture();

        public void Setup()
        {
            MessageStorage.Clear();
        }

        public async Task JsonGzipMessageTest(IMessageProducer<JsonGzipProducer> producer)
        {
            // Arrange
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            messages.ForEach(async m => await MessageStorage.AssertMessageEqualAsync(m));
        }
        
        public async Task ProtoBufGzipMessageTest(IMessageProducer<ProtobufGzipProducer> producer)
        {
            // Arrange
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            messages.ForEach(async m => await MessageStorage.AssertMessageEqualAsync(m));
        }
    }
}

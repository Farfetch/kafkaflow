namespace KafkaFlow.IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common.Core.Handlers;
    using KafkaFlow.IntegrationTests.Common.Core.Messages;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using Producers;

    [TestClass]
    public class BaseSerializationTest
    {
        private readonly Fixture fixture = new Fixture();
        
        public void Setup()
        {
            MessageStorage.Clear();
        }

        public async Task JsonMessageTest(IMessageProducer<JsonProducer> producer)
        {
            // Arrange
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

            // Act
            await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            messages.ForEach(async m => await MessageStorage.AssertMessageEqualAsync(m));
        }

        public async Task ProtobufMessageTest(IMessageProducer<ProtobufProducer> producer)
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

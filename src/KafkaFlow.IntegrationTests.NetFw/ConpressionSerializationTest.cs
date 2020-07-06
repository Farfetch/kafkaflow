namespace KafkaFlow.IntegrationTests.NetFw
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Core.Handlers;
    using Core.Messages;
    using Core.Producers;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::Unity;
    using KafkaFlow.Producers;

    [TestClass]
    public class CompressionSerializationTest
    {
        private UnityContainer container;
        private readonly Fixture fixture = new Fixture();

        [TestInitialize]
        public void Setup()
        {
            this.container = TestSetup.GetContainer();
            MessageStorage.Clear();
        }

        [TestMethod]
        public async Task JsonGzipMessageTest()
        {
            // Arrange
            var producer = this.container.Resolve<IMessageProducer<JsonGzipProducer>>();
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
        public async Task ProtoBufGzipMessageTest()
        {
            // Arrange
            var producer = this.container.Resolve<IMessageProducer<ProtobufGzipProducer>>();
            var messages = this.fixture.CreateMany<TestMessage1>(10).ToList();

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

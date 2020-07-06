namespace KafkaFlow.IntegrationTests.NetFw
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Core.Handlers;
    using Core.Producers;
    using global::Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Producers;

    [TestClass]
    public class CompressionTest
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
        public async Task GzipTest()
        {
            // Arrange
            var producer = this.container.Resolve<IMessageProducer<GzipProducer>>();
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

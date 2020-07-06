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
    public class ConsumerTest
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
        public async Task MultipleMessagesMultipleHandlersSingleTopicTest()
        {
            // Arrange
            var producer = this.container.Resolve<IMessageProducer<JsonProducer>>();
            var messages1 = this.fixture.CreateMany<TestMessage1>(5).ToList();
            var messages2 = this.fixture.CreateMany<TestMessage2>(5).ToList();
            
            // Act
            await Task.WhenAll(messages1.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));
            await Task.WhenAll(messages2.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

            // Assert
            foreach (var message in messages1)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
            
            foreach (var message in messages2)
            {
                await MessageStorage.AssertMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task MultipleTopicsSingleConsumerTest()
        {
            // Arrange
            var producer1 = this.container.Resolve<IMessageProducer<ProtobufGzipProducer>>();
            var producer2 = this.container.Resolve<IMessageProducer<ProtobufGzipProducer2>>();
            var messages = this.fixture.CreateMany<TestMessage1>(1).ToList();

            // Act
            messages.ForEach(m => producer1.Produce(m.Id.ToString(), m));
            messages.ForEach(m => producer2.Produce(m.Id.ToString(), m));

            // Assert
            foreach (var message in messages)
            {
                await MessageStorage.AssertCountMessageAsync(message, 2);
            }
        }

        [TestMethod]
        public async Task MessageOrderingTest()
        {
            // Arrange
            var version = 1;
            var partitionKey = Guid.NewGuid();
            var producer = this.container.Resolve<IMessageProducer<ProtobufProducer>>();
            var messages = this.fixture
                .Build<TestMessage1>()
                .Without(t => t.Version)
                .Do(t => t.Version = version++)
                .CreateMany(5)
                .ToList();

            // Act
            foreach (var m in messages)
            {
                await producer.ProduceAsync(partitionKey.ToString(), m);
            }

            // Assert
            await Task.Delay(10000);

            var versionsSent = messages.Select(m => m.Version).ToList();
            var versionsReceived = MessageStorage
                    .GetVersions()
                    .OrderBy(r => r.ticks)
                    .Select(r => r.version)
                    .ToList();

            CollectionAssert.AreEqual(versionsSent, versionsReceived, $"sent: {string.Join(",",versionsSent)} | received: {string.Join(", ",versionsReceived)} | ");
        }
    }
}

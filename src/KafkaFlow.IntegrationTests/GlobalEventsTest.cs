namespace KafkaFlow.IntegrationTests
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using KafkaFlow.Configuration;
    using KafkaFlow.IntegrationTests.Core;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Middlewares;
    using KafkaFlow.IntegrationTests.Core.Producers;
    using KafkaFlow.Serializer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GlobalEventsTest
    {
        private readonly Fixture fixture = new();

        [TestMethod]
        public async Task SubscribeGlobalEvents_AllEvents_TriggeredCorrectly()
        {
            // Arrange
            bool isMessageProducedStarted = false, isMessageConsumeStarted = false, isMessageConsumeCompleted = false;

            void ConfigureGlobalEvents(IGlobalEvents observers)
            {
                observers.MessageProduceStarted.Subscribe(eventContext =>
                {
                    isMessageProducedStarted = true;
                    return Task.CompletedTask;
                });

                observers.MessageConsumeStarted.Subscribe(eventContext =>
                {
                    isMessageConsumeStarted = true;
                    return Task.CompletedTask;
                });

                observers.MessageConsumeCompleted.Subscribe(eventContext =>
                {
                    isMessageConsumeCompleted = true;
                    return Task.CompletedTask;
                });
            }

            var provider = this.GetServiceProvider(ConfigureGlobalEvents);
            MessageStorage.Clear();

            var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
            var message = this.fixture.Create<byte[]>();

            // Act
            await producer.ProduceAsync(null, message);

            await MessageStorage.AssertMessageAsync(message);

            // Assert
            Assert.IsTrue(isMessageProducedStarted);
            Assert.IsTrue(isMessageConsumeStarted);
            Assert.IsTrue(isMessageConsumeCompleted);
        }

        [TestMethod]
        public void SubscribeGlobalEvents_MessageContext_IsAssignedCorrectly()
        {
            // Arrange
            IMessageContext messageContext = null;

            void ConfigureGlobalEvents(IGlobalEvents observers)
            {
                observers.MessageProduceStarted.Subscribe(eventContext =>
                {
                    messageContext = eventContext.MessageContext;
                    return Task.CompletedTask;
                });
            }

            var provider = this.GetServiceProvider(ConfigureGlobalEvents);
            MessageStorage.Clear();

            var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
            var message = this.fixture.Create<TestMessage1>();

            // Act
            producer.Produce(message.Id.ToString(), message);

            // Assert
            Assert.IsNotNull(messageContext);
            Assert.AreEqual(messageContext.Message.Key, message.Id.ToString());
        }

        private IServiceProvider GetServiceProvider(Action<IGlobalEvents> configureGlobalEvents)
        {
            var topicName = $"topic_{Guid.NewGuid()}";

            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                    (_, config) =>
                    {
                        config
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(
                                "conf/appsettings.json",
                                false,
                                true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureServices((context, services) =>
                    services.AddKafka(
                        kafka => kafka
                            .UseLogHandler<TraceLogHandler>()
                            .AddCluster(
                                cluster => cluster
                                    .WithBrokers(context.Configuration.GetValue<string>("Kafka:Brokers").Split(';'))
                                    .CreateTopicIfNotExists(topicName, 1, 1)
                                    .AddProducer<JsonProducer2>(
                                        producer => producer
                                            .DefaultTopic(topicName)
                                            .AddMiddlewares(
                                                middlewares => middlewares
                                                    .AddSerializer<ProtobufNetSerializer>()))
                                    .AddConsumer(
                                        consumer => consumer
                                            .Topic(topicName)
                                            .WithGroupId(topicName)
                                            .WithBufferSize(100)
                                            .WithWorkersCount(10)
                                            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                                            .AddMiddlewares(
                                                middlewares => middlewares
                                                    .AddSerializer<ProtobufNetSerializer>()
                                                    .Add<GzipMiddleware>())))
                            .SubscribeGlobalEvents(configureGlobalEvents)))
                .UseDefaultServiceProvider(
                    (_, options) =>
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    });

            var host = builder.Build();
            var bus = host.Services.CreateKafkaBus();
            bus.StartAsync().GetAwaiter().GetResult();

            // Wait partition assignment
            Thread.Sleep(10000);

            return host.Services;
        }
    }
}

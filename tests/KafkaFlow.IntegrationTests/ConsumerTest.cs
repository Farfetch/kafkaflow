using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Confluent.Kafka;
using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaFlow.Consumers;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Messages;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.Serializer;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class ConsumerTest
{
    private readonly Fixture _fixture = new();

    private IServiceProvider _provider;

    [TestInitialize]
    public void Setup()
    {
        _provider = Bootstrapper.GetServiceProvider();
        MessageStorage.Clear();
    }

    [TestMethod]
    public async Task MultipleMessagesMultipleHandlersSingleTopicTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<JsonProducer>>();
        var messages1 = _fixture.CreateMany<TestMessage1>(5).ToList();
        var messages2 = _fixture.CreateMany<TestMessage2>(5).ToList();

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
        var producer1 = _provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
        var producer2 = _provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer2>>();
        var messages = _fixture.CreateMany<TestMessage1>(1).ToList();

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
    public async Task MultipleHandlersSingleTypeConsumerTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<JsonProducer>>();
        var messages = _fixture.CreateMany<TestMessage1>(5).ToList();

        // Act
        messages.ForEach(m => producer.Produce(m.Id.ToString(), m));

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
        var producer = _provider.GetRequiredService<IMessageProducer<ProtobufProducer>>();
        var messages = _fixture
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
        await Task.Delay(8000).ConfigureAwait(false);
        var versionsSent = messages.Select(m => m.Version).ToList();
        var versionsReceived = MessageStorage
            .GetVersions()
            .OrderBy(r => r.ticks)
            .Select(r => r.version)
            .ToList();

        CollectionAssert.AreEqual(versionsSent, versionsReceived);
    }

    [TestMethod]
    public async Task PauseResumeHeartbeatTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<ProtobufProducer>>();
        var messages = _fixture.CreateMany<PauseResumeMessage>(5).ToList();

        // Act
        await Task.WhenAll(
            messages.Select(
                m => producer.ProduceAsync(
                    Bootstrapper.PauseResumeTopicName,
                    m.Id.ToString(),
                    m)));

        await Task.Delay(40000);

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public void AddConsumer_WithSharedConsumerConfig_ConsumersAreConfiguratedIndependently()
    {
        // Act
        var consumers = _provider.GetRequiredService<IConsumerAccessor>().All;

        // Assert
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.AvroGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.GzipGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.JsonGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.JsonGzipGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.PauseResumeGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.ProtobufGroupId)));
        Assert.IsNotNull(consumers.FirstOrDefault(x => x.GroupId.Equals(Bootstrapper.ProtobufGzipGroupId)));
    }

    [TestMethod]
    public async Task ManualAssignPartitionOffsetsTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<OffsetTrackerProducer>>();
        var messages = _fixture
            .Build<OffsetTrackerMessage>()
            .Without(m => m.Offset)
            .CreateMany(10).ToList();

        messages.ForEach(m => producer.Produce(m.Id.ToString(), m, null, DeliveryHandler));

        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
        
        var endOffset = MessageStorage.GetOffsetTrack();
        MessageStorage.Clear();
        
        // Act
        var serviceProviderHelper = new ServiceProviderHelper();
        
        await serviceProviderHelper.GetServiceProviderAsync(
            consumerConfig =>
            {
                consumerConfig.ManualAssignPartitionOffsets(Bootstrapper.OffsetTrackerTopicName, new Dictionary<int, long> { { 0, endOffset - 4 } })
                    .WithGroupId("ManualAssignPartitionOffsetsTest")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(
                        middlewares => middlewares
                            .AddDeserializer<JsonCoreDeserializer>()
                            .AddTypedHandlers(
                                handlers => handlers.AddHandler<OffsetTrackerMessageHandler>()));
            }, null);
        
        // Assert
        for (var i = 0; i < 5; i++)
        {
            await MessageStorage.AssertOffsetTrackerMessageAsync(messages[i], false);
        }
        
        for (var i = 5; i < 10; i++)
        {
            await MessageStorage.AssertOffsetTrackerMessageAsync(messages[i]);
        }

        await serviceProviderHelper.StopBusAsync();
        
        return;
        
        void DeliveryHandler(DeliveryReport<byte[], byte[]> report)
        {
            var key = Encoding.UTF8.GetString(report.Message.Key);
            var message = messages.First(m => m.Id.ToString() == key);
            message.Offset = report.Offset;
        }
    }
}

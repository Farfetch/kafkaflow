using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Messages;
using KafkaFlow.IntegrationTests.Core.Producers;
using MessageTypes;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class SerializationTest
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
    public async Task JsonMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<JsonProducer>>();
        var messages = _fixture.CreateMany<TestMessage1>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public async Task ProtobufMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<ProtobufProducer>>();
        var messages = _fixture.CreateMany<TestMessage1>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public async Task AvroMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<AvroProducer>>();
        var messages = _fixture.CreateMany<LogMessages2>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public async Task ProtobufSchemaRegistryMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<ConfluentProtobufProducer>>();
        var messages = _fixture.CreateMany<TestProtoMessage>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id, m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public async Task JsonSchemaRegistryMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<ConfluentJsonProducer>>();
        var messages = _fixture.CreateMany<TestMessage3>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }

    [TestMethod]
    public async Task AvroConvertMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<AvroConvertProducer>>();
        var messages = _fixture.CreateMany<TestAvroConvertMessage>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }
}

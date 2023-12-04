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

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class CompressionSerializationTest
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
    public async Task JsonGzipMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<JsonGzipProducer>>();
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
    public async Task ProtoBufGzipMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<ProtobufGzipProducer>>();
        var messages = _fixture.CreateMany<TestMessage1>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(m.Id.ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }
}

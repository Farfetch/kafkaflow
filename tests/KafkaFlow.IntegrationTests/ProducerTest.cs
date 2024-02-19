using System;
using System.Threading.Tasks;
using AutoFixture;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class ProducerTest
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
    public async Task ProduceNullKeyTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        // Act
        await producer.ProduceAsync(null, message);

        // Assert
        await MessageStorage.AssertMessageAsync(message);
    }
    
    [TestMethod]
    public async Task ProduceNullMessageTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<NullProducer>>();
        var key = Guid.NewGuid().ToString();

        // Act
        await producer.ProduceAsync(key, null);

        // Assert
        await MessageStorage.AssertNullMessageAsync();
    }
}

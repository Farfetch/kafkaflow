using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Producers;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class CompressionTest
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
    public async Task GzipTest()
    {
        // Arrange
        var producer = _provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var messages = _fixture.CreateMany<byte[]>(10).ToList();

        // Act
        await Task.WhenAll(messages.Select(m => producer.ProduceAsync(Guid.NewGuid().ToString(), m)));

        // Assert
        foreach (var message in messages)
        {
            await MessageStorage.AssertMessageAsync(message);
        }
    }
}

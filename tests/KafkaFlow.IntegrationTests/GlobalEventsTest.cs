﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Confluent.Kafka;
using KafkaFlow.Configuration;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Exceptions;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Messages;
using KafkaFlow.IntegrationTests.Core.Middlewares;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class GlobalEventsTest
{
    private readonly Fixture _fixture = new();
    private string _topic;
    private bool _isPartitionAssigned;
    private IKafkaBus _bus;

    [TestInitialize]
    public void Setup()
    {
        _topic = $"GlobalEventsTestTopic_{Guid.NewGuid()}";

        MessageStorage.Clear();
    }

    [TestMethod]
    public async Task OnStarted_RegisterMultipleOnStartedCallbacks_AllAreCalled()
    {
        // Arrange
        const int ExpectedOnStartedCount = 2;
        var countOnStarted = 0;

        // Act
        await this.GetServiceProviderAsync(
            observers => { },
            this.ConfigureConsumer<GzipMiddleware>,
            this.ConfigureProducer<ProtobufNetSerializer>,
            cluster =>
            {
                cluster.OnStarted(_ => countOnStarted++);
                cluster.OnStarted(_ => countOnStarted++);
            });

        // Assert
        Assert.AreEqual(ExpectedOnStartedCount, countOnStarted);
    }

    [TestMethod]
    public async Task OnStopping_RegisterMultipleOnStoppingCallbacks_AllAreCalled()
    {
        // Arrange
        const int ExpectedOnStoppingCount = 2;
        var countOnStopping = 0;

        // Act
        await this.GetServiceProviderAsync(
            observers => { },
            this.ConfigureConsumer<GzipMiddleware>,
            this.ConfigureProducer<ProtobufNetSerializer>,
            cluster =>
            {
                cluster.OnStopping(_ => countOnStopping++);
                cluster.OnStopping(_ => countOnStopping++);
            });

        await _bus?.StopAsync();

        // Assert
        Assert.AreEqual(ExpectedOnStoppingCount, countOnStopping);
    }

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

        var provider = await this.GetServiceProviderAsync(
            ConfigureGlobalEvents,
            this.ConfigureConsumer<GzipMiddleware>,
            this.ConfigureProducer<ProtobufNetSerializer>);
        MessageStorage.Clear();

        var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
        var message = _fixture.Create<byte[]>();

        // Act
        await producer.ProduceAsync(null, message);

        await MessageStorage.AssertMessageAsync(message);

        // Assert
        Assert.IsTrue(isMessageProducedStarted);
        Assert.IsTrue(isMessageConsumeStarted);
        Assert.IsTrue(isMessageConsumeCompleted);
    }

    [TestMethod]
    public async Task SubscribeGlobalEvents_MessageContext_IsAssignedCorrectly()
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

        var provider = await this.GetServiceProviderAsync(
            ConfigureGlobalEvents,
            this.ConfigureConsumer<GzipMiddleware>,
            this.ConfigureProducer<ProtobufNetSerializer>);

        MessageStorage.Clear();

        var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
        var message = _fixture.Create<TestMessage1>();

        // Act
        producer.Produce(message.Id.ToString(), message);

        // Assert
        Assert.IsNotNull(messageContext);
        Assert.AreEqual(messageContext.Message.Key, message.Id.ToString());
    }

    [TestMethod]
    public async Task SubscribeGlobalEvents_ConsumerErrorEvent_TriggeredCorrectly()
    {
        // Arrange
        bool isMessageProducedStarted = false, isMessageConsumeStarted = false, isMessageConsumerError = false;

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

            observers.MessageConsumeError.Subscribe(eventContext =>
            {
                isMessageConsumerError = true;
                return Task.CompletedTask;
            });
        }

        var provider = await this.GetServiceProviderAsync(
            ConfigureGlobalEvents,
            this.ConfigureConsumer<TriggerErrorMessageMiddleware>,
            this.ConfigureProducer<ProtobufNetSerializer>);

        MessageStorage.Clear();

        var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
        var message = _fixture.Create<byte[]>();

        // Act
        await producer.ProduceAsync(null, message);

        await MessageStorage.AssertMessageAsync(message);

        // Assert
        Assert.IsTrue(isMessageProducedStarted);
        Assert.IsTrue(isMessageConsumeStarted);
        Assert.IsTrue(isMessageConsumerError);
    }

    [TestMethod]
    public async Task SubscribeGlobalEvents_ProducerErrorEvent_TriggeredCorrectly()
    {
        // Arrange
        bool isMessageProducedStarted = false, isProduceErrorFired = false;

        void ConfigureGlobalEvents(IGlobalEvents observers)
        {
            observers.MessageProduceStarted.Subscribe(eventContext =>
            {
                isMessageProducedStarted = true;
                return Task.CompletedTask;
            });

            observers.MessageProduceError.Subscribe(eventContext =>
            {
                isProduceErrorFired = true;
                return Task.CompletedTask;
            });
        }

        var provider = await this.GetServiceProviderAsync(
            ConfigureGlobalEvents,
            this.ConfigureConsumer<TriggerErrorMessageMiddleware>,
            this.ConfigureProducer<TriggerErrorSerializerMiddleware>);

        MessageStorage.Clear();
        var producer = provider.GetRequiredService<IMessageProducer<JsonProducer2>>();
        var message = _fixture.Create<byte[]>();
        var errorOccured = false;

        // Act
        try
        {
            await producer.ProduceAsync(null, message);
        }
        catch (ProduceException<byte[], byte[]> ex)
        {
            // Assert
            errorOccured = true;
            Assert.IsTrue(ex.GetType() == typeof(ProduceException<byte[], byte[]>));
            Assert.IsTrue(isMessageProducedStarted);
            Assert.IsTrue(isProduceErrorFired);
        }

        Assert.IsTrue(errorOccured);
    }

    private void ConfigureConsumer<T>(IConsumerConfigurationBuilder consumerConfigurationBuilder)
        where T : class, IMessageMiddleware
    {
        consumerConfigurationBuilder
            .Topic(_topic)
            .WithGroupId(_topic)
            .WithBufferSize(100)
            .WithWorkersCount(10)
            .WithAutoOffsetReset(KafkaFlow.AutoOffsetReset.Earliest)
            .AddMiddlewares(
                middlewares => middlewares
                    .AddDeserializer<ProtobufNetDeserializer>()
                    .Add<T>())
            .WithPartitionsAssignedHandler((_, _) =>
            {
                _isPartitionAssigned = true;
            });
    }

    private void ConfigureProducer<T>(IProducerConfigurationBuilder producerConfigurationBuilder)
        where T : class, ISerializer
    {
        producerConfigurationBuilder
            .DefaultTopic(_topic)
            .AddMiddlewares(middlewares => middlewares.AddSerializer<T>());
    }

    private async Task<IServiceProvider> GetServiceProviderAsync(
        Action<IGlobalEvents> configureGlobalEvents,
        Action<IConsumerConfigurationBuilder> consumerConfiguration,
        Action<IProducerConfigurationBuilder> producerConfiguration,
        Action<IClusterConfigurationBuilder> builderConfiguration = null)
    {
        _isPartitionAssigned = false;

        var clusterBuilderAction = (HostBuilderContext context, IClusterConfigurationBuilder cluster) =>
        {
            cluster
                .WithBrokers(context.Configuration.GetValue<string>("Kafka:Brokers").Split(';'))
                .CreateTopicIfNotExists(_topic, 1, 1)
                .AddProducer<JsonProducer2>(producerConfiguration)
                .AddConsumer(consumerConfiguration);
        };

        clusterBuilderAction += (_, cluster) =>
        {
            builderConfiguration?.Invoke(cluster);
        };

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
                        .AddCluster(cluster => { clusterBuilderAction.Invoke(context, cluster); })
                        .SubscribeGlobalEvents(configureGlobalEvents)))
            .UseDefaultServiceProvider(
                (_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

        var host = builder.Build();
        _bus = host.Services.CreateKafkaBus();
        _bus.StartAsync().GetAwaiter().GetResult();

        await this.WaitForPartitionAssignmentAsync();

        return host.Services;
    }

    private async Task WaitForPartitionAssignmentAsync()
    {
        await Policy
            .HandleResult<bool>(isAvailable => !isAvailable)
            .WaitAndRetryAsync(Enumerable.Range(0, 6).Select(i => TimeSpan.FromSeconds(Math.Pow(i, 2))))
            .ExecuteAsync(() => Task.FromResult(_isPartitionAssigned));
    }

    private class TriggerErrorMessageMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate _)
        {
            MessageStorage.Add((byte[])context.Message.Value);
            throw new ErrorExecutingMiddlewareException(nameof(TriggerErrorMessageMiddleware));
        }
    }

    private class TriggerErrorSerializerMiddleware : ISerializer
    {
        public Task SerializeAsync(object _, Stream output, ISerializerContext context)
        {
            var error = new Error(ErrorCode.BrokerNotAvailable);
            throw new ProduceException<byte[], byte[]>(error, null);
        }

        public Task<object> DeserializeAsync(Stream _, Type type, ISerializerContext context)
        {
            var error = new Error(ErrorCode.BrokerNotAvailable);
            throw new ProduceException<byte[], byte[]>(error, null);
        }
    }
}

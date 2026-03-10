using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using OpenTelemetry;
using OpenTelemetry.Trace;
using KafkaFlow.Compressor.Gzip;
using KafkaFlow.Configuration;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Middlewares;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class OpenTelemetryTests
{
    private readonly Fixture _fixture = new();

    private List<Activity> _exportedItems;

    private bool _isPartitionAssigned;

    private string _topicName;

    [TestInitialize]
    public void Setup()
    {
        _exportedItems = new List<Activity>();
        _topicName =  $"OpenTelemetryTestTopic_{Guid.NewGuid()}";
    }

    [TestMethod]
    public async Task AddOpenTelemetry_ProducingAndConsumingOneMessage_TraceAndSpansAreCreatedCorrectly()
    {
        // Arrange
        var provider = await this.GetServiceProvider();
        MessageStorage.Clear();

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        // Act
        await producer.ProduceAsync(null, message);

        // Assert
        var (producerSpan, consumerSpan, internalSpan) = await this.WaitForSpansAsync();

        Assert.IsNotNull(_exportedItems);
        Assert.IsNull(producerSpan.ParentId);
        Assert.AreEqual(producerSpan.TraceId, consumerSpan.TraceId);
        Assert.AreEqual(consumerSpan.ParentSpanId, producerSpan.SpanId);
    }

    [TestMethod]
    public async Task AddOpenTelemetry_WithEnrichProducer_DefinedHeaderIsIncludedInActivity()
    {
        // Arrange
        string tagName = "otel.header.tag.name";
        string headerKey = "otel-header-key";
        string headerValue = "otel-header-value";
        var provider = await this.GetServiceProvider(options => options.EnrichProducer = (activity, messageContext) => 
        {
            var header = messageContext.Headers.FirstOrDefault(x => x.Key == headerKey);
            var valueString = Encoding.UTF8.GetString(header.Value);
            activity.SetTag(tagName, valueString);
        });

        MessageStorage.Clear();

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();
        IMessageHeaders headers = new MessageHeaders
        {
            { headerKey, Encoding.UTF8.GetBytes(headerValue) }
        };

        // Act
        await producer.ProduceAsync(_topicName, "key", message, headers);

        // Assert
        var (producerSpan, consumerSpan, internalSpan) = await this.WaitForSpansAsync();

        var otelTagValueProducer = producerSpan.Tags.FirstOrDefault(x => x.Key == tagName).Value;
        var otelTagValueConsumer = consumerSpan.Tags.FirstOrDefault(x => x.Key == tagName).Value;

        Assert.IsNotNull(_exportedItems);
        Assert.IsNull(otelTagValueConsumer);
        Assert.IsNotNull(otelTagValueProducer);
        Assert.AreEqual(otelTagValueProducer, headerValue);
    }

    [TestMethod]
    public async Task AddOpenTelemetry_WithEnrichConsumer_DefinedHeaderIsIncludedInActivity()
    {
        // Arrange
        string tagName = "otel-header-tag-name";
        string headerKey = "otel-header-key";
        string headerValue = "otel-header-value";
        var provider = await this.GetServiceProvider(options => options.EnrichConsumer = (activity, messageContext) =>
        {
            var header = messageContext.Headers.FirstOrDefault(x => x.Key == headerKey);
            var valueString = Encoding.UTF8.GetString(header.Value);
            activity.SetTag(tagName, valueString);
        });

        MessageStorage.Clear();

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();
        IMessageHeaders headers = new MessageHeaders
        {
            { headerKey, Encoding.UTF8.GetBytes(headerValue) }
        };

        // Act
        await producer.ProduceAsync(_topicName, "key", message, headers);

        // Assert
        var (producerSpan, consumerSpan, internalSpan) = await this.WaitForSpansAsync();

        var otelTagValueProducer = producerSpan.Tags.FirstOrDefault(x => x.Key == tagName).Value;
        var otelTagValueConsumer = consumerSpan.Tags.FirstOrDefault(x => x.Key == tagName).Value;

        Assert.IsNotNull(_exportedItems);
        Assert.IsNull(otelTagValueProducer);
        Assert.IsNotNull(otelTagValueConsumer);
        Assert.AreEqual(otelTagValueConsumer, headerValue);
    }

    [TestMethod]
    public async Task AddOpenTelemetry_CreateActivityOnConsumingMessage_TraceIsPropagatedToCreatedActivity()
    {
        // Arrange
        var provider = await this.GetServiceProvider();
        MessageStorage.Clear();

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource("KafkaFlow.OpenTelemetry")
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        // Act
        await producer.ProduceAsync(null, message);

        // Assert
        var (producerSpan, consumerSpan, internalSpan) = await this.WaitForSpansAsync();

        Assert.IsNotNull(_exportedItems);
        Assert.IsNull(producerSpan.ParentId);
        Assert.AreEqual(producerSpan.TraceId, consumerSpan.TraceId);
        Assert.AreEqual(consumerSpan.ParentSpanId, producerSpan.SpanId);
        Assert.AreEqual(internalSpan.TraceId, consumerSpan.TraceId);
        Assert.AreEqual(internalSpan.ParentSpanId, consumerSpan.SpanId);
    }

    [TestMethod]
    public async Task AddOpenTelemetry_ProducingAndConsumingOneMessage_BaggageIsPropagatedFromTestActivityToConsumer()
    {
        // Arrange
        var provider = await this.GetServiceProvider();
        MessageStorage.Clear();

        var kafkaFlowTestString = "KafkaFlowTest";
        var baggageName1 = "TestBaggage1";
        var baggageValue1 = "TestBaggageValue1";
        var baggageName2 = "TestBaggage2";
        var baggageValue2 = "TestBaggageValue2";

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddSource(kafkaFlowTestString)
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        // Act
        ActivitySource activitySource = new(kafkaFlowTestString);

        var activity = activitySource.StartActivity("TestActivity", ActivityKind.Client);

        activity.AddBaggage(baggageName1, baggageValue1);
        activity.AddBaggage(baggageName2, baggageValue2);

        await producer.ProduceAsync(null, message);

        // Assert
        var (producerSpan, consumerSpan, internalSpan) = await this.WaitForSpansAsync();

        Assert.IsNotNull(_exportedItems);
        Assert.AreEqual(producerSpan.TraceId, consumerSpan.TraceId);
        Assert.AreEqual(consumerSpan.ParentSpanId, producerSpan.SpanId);
        Assert.AreEqual(producerSpan.GetBaggageItem(baggageName1), baggageValue1);
        Assert.AreEqual(consumerSpan.GetBaggageItem(baggageName1), baggageValue1);
        Assert.AreEqual(producerSpan.GetBaggageItem(baggageName2), baggageValue2);
        Assert.AreEqual(consumerSpan.GetBaggageItem(baggageName2), baggageValue2);
    }

    [TestMethod]
    public async Task AddOpenTelemetry_ProducingMessage_BaggageCurrentMergedWithActivityBaggage()
    {
        // Arrange
        var provider = await this.GetServiceProvider();
        MessageStorage.Clear();

        var baggageName1 = "CurrentBaggageKey";
        var baggageValue1 = "CurrentBaggageValue";
        var baggageName2 = "ActivityBaggageKey";
        var baggageValue2 = "ActivityBaggageValue";
        var baggageNameConflict = "ConflictKey";
        var currentValue = "CurrentValue";
        var activityValue = "ActivityValue";

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
        .AddInMemoryExporter(_exportedItems)
        .Build();

        var producer = provider.GetRequiredService<IMessageProducer<GzipProducer>>();
        var message = _fixture.Create<byte[]>();

        // Set initial Baggage.Current values
        Baggage.SetBaggage(baggageName1, baggageValue1);
        Baggage.SetBaggage(baggageNameConflict, currentValue);

        // Act: create Activity with baggage (per OTel spec, Activity baggage has precedence on conflicts)
        var activitySource = new ActivitySource(KafkaFlowInstrumentation.ActivitySourceName);
        using (var activity = activitySource.StartActivity("TestActivity", ActivityKind.Producer))
        {
            activity?.AddBaggage(baggageName2, baggageValue2);
            activity?.AddBaggage(baggageNameConflict, activityValue); // should overwrite
            await producer.ProduceAsync(null, message);
        }

        // Assert
        var (_, consumerSpan, _) = await this.WaitForSpansAsync();

        // Current baggage values must be preserved
        Assert.AreEqual(baggageValue1, consumerSpan.GetBaggageItem(baggageName1));

        // Activity baggage values must be propagated
        Assert.AreEqual(baggageValue2, consumerSpan.GetBaggageItem(baggageName2));

        // On conflict, Activity baggage must take precedence according to OTel spec
        Assert.AreEqual(activityValue, consumerSpan.GetBaggageItem(baggageNameConflict));
    }

    private async Task<IServiceProvider> GetServiceProvider(Action<KafkaFlowInstrumentationOptions> options = null)
    {
        

        _isPartitionAssigned = false;

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
                                .CreateTopicIfNotExists(_topicName, 1, 1)
                                .AddProducer<GzipProducer>(
                                    producer => producer
                                        .DefaultTopic(_topicName)
                                        .AddMiddlewares(
                                            middlewares => middlewares
                                                .AddCompressor<GzipMessageCompressor>()))
                                .AddConsumer(
                                    consumer => consumer
                                        .Topic(_topicName)
                                        .WithGroupId(_topicName)
                                        .WithBufferSize(100)
                                        .WithWorkersCount(10)
                                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                        .AddMiddlewares(
                                            middlewares => middlewares
                                                .AddDecompressor<GzipMessageDecompressor>()
                                                .Add<GzipMiddleware>())
                                        .WithPartitionsAssignedHandler((_, _) =>
                                        {
                                            _isPartitionAssigned = true;
                                        })))
                        .AddOpenTelemetryInstrumentation(options)))
            .UseDefaultServiceProvider(
                (_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

        var host = builder.Build();
        var bus = host.Services.CreateKafkaBus();
        bus.StartAsync().GetAwaiter().GetResult();

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

    private async Task<(Activity producerSpan, Activity consumerSpan, Activity internalSpan)> WaitForSpansAsync()
    {
        Activity producerSpan = null, consumerSpan = null, internalSpan = null;

        await Policy
            .HandleResult<bool>(isAvailable => !isAvailable)
            .WaitAndRetryAsync(Enumerable.Range(0, 6).Select(i => TimeSpan.FromSeconds(Math.Pow(i, 2))))
            .ExecuteAsync(() =>
            {
                producerSpan = _exportedItems.Find(x => x.Kind == ActivityKind.Producer);
                consumerSpan = _exportedItems.Find(x => x.Kind == ActivityKind.Consumer);
                internalSpan = _exportedItems.Find(x => x.Kind == ActivityKind.Internal);

                return Task.FromResult(producerSpan != null && consumerSpan != null);
            });

        return (producerSpan, consumerSpan, internalSpan);
    }
}
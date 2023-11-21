using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using global::OpenTelemetry;
using global::OpenTelemetry.Trace;
using KafkaFlow.Compressor.Gzip;
using KafkaFlow.Configuration;
using KafkaFlow.IntegrationTests.Core;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Middlewares;
using KafkaFlow.IntegrationTests.Core.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

namespace KafkaFlow.IntegrationTests
{
    [TestClass]
    public class OpenTelemetryTests
    {
        private readonly Fixture _fixture = new();

        private List<Activity> _exportedItems;

        private bool _isPartitionAssigned;

        [TestInitialize]
        public void Setup()
        {
            _exportedItems = new List<Activity>();
        }

        [TestMethod]
        public async Task AddOpenTelemetry_ProducingAndConsumingOneMessage_TraceAndSpansAreCreatedCorrectly()
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
            var (producerSpan, consumerSpan) = await this.WaitForSpansAsync();

            Assert.IsNotNull(_exportedItems);
            Assert.IsNull(producerSpan.ParentId);
            Assert.AreEqual(producerSpan.TraceId, consumerSpan.TraceId);
            Assert.AreEqual(consumerSpan.ParentSpanId, producerSpan.SpanId);
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
            .AddSource("KafkaFlow.OpenTelemetry")
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
            var (producerSpan, consumerSpan) = await this.WaitForSpansAsync();

            Assert.IsNotNull(_exportedItems);
            Assert.AreEqual(producerSpan.TraceId, consumerSpan.TraceId);
            Assert.AreEqual(consumerSpan.ParentSpanId, producerSpan.SpanId);
            Assert.AreEqual(producerSpan.GetBaggageItem(baggageName1), baggageValue1);
            Assert.AreEqual(consumerSpan.GetBaggageItem(baggageName1), baggageValue1);
            Assert.AreEqual(producerSpan.GetBaggageItem(baggageName2), baggageValue2);
            Assert.AreEqual(consumerSpan.GetBaggageItem(baggageName2), baggageValue2);
        }

        private async Task<IServiceProvider> GetServiceProvider()
        {
            var topicName = $"OpenTelemetryTestTopic_{Guid.NewGuid()}";

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
                                    .CreateTopicIfNotExists(topicName, 1, 1)
                                    .AddProducer<GzipProducer>(
                                        producer => producer
                                            .DefaultTopic(topicName)
                                            .AddMiddlewares(
                                                middlewares => middlewares
                                                    .AddCompressor<GzipMessageCompressor>()))
                                    .AddConsumer(
                                        consumer => consumer
                                            .Topic(topicName)
                                            .WithGroupId(topicName)
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
                            .AddOpenTelemetryInstrumentation()))
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

        private async Task<(Activity producerSpan, Activity consumerSpan)> WaitForSpansAsync()
        {
            Activity producerSpan = null, consumerSpan = null;

            await Policy
                .HandleResult<bool>(isAvailable => !isAvailable)
                .WaitAndRetryAsync(Enumerable.Range(0, 6).Select(i => TimeSpan.FromSeconds(Math.Pow(i, 2))))
                .ExecuteAsync(() =>
                {
                    producerSpan = _exportedItems.Find(x => x.Kind == ActivityKind.Producer);
                    consumerSpan = _exportedItems.Find(x => x.Kind == ActivityKind.Consumer);

                    return Task.FromResult(producerSpan != null && consumerSpan != null);
                });

            return (producerSpan, consumerSpan);
        }
    }
}

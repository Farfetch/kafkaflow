using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Sample.CooperativeSticky;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

const string producerName = "PrintConsole";
const string topicName = "sample-topic";
var hostBuilder = new HostBuilder();
hostBuilder.ConfigureServices(services =>
    services.AddHostedService<HostedService>().AddKafka(
        kafka => kafka
            .UseConsoleLog()
            .AddCluster(
                cluster => cluster
                    .WithBrokers(new[] { "localhost:9092" })
                    .CreateTopicIfNotExists(topicName, 6, 1)
                    .AddProducer(
                        producerName,
                        producer => producer
                            .DefaultTopic(topicName)
                            .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                    )
                    .AddConsumer(
                        consumer => consumer
                            .WithConsumerConfig(new ConsumerConfig
                            {
                                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky})
                            .Topic(topicName)
                            .WithGroupId("print-console-handler")
                            .WithBufferSize(100)
                            .WithWorkersCount(3)
                            .WithAutoCommitIntervalMs(100)
                            .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            .AddMiddlewares(
                                middlewares => middlewares
                                    .AddDeserializer<ProtobufNetDeserializer>()
                                    .AddTypedHandlers(h => h.AddHandler<PrintConsoleHandler>())
                            )
                    )
            )
    ));

var build = hostBuilder.Build();
var kafkaBus = build.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await build.RunAsync();
await kafkaBus.StopAsync();
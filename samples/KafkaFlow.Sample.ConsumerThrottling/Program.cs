/*
The key feature demonstrated in this sample is the ThrottleConsumer middleware configured on "consumerB". This middleware monitors the lag
of "consumerA" and applies penalties to "consumerB" if "consumerA" falls behind.

The ThrottleConsumer mechanism works by checking the lag of "consumerA" every 1 seconds.
If a lag of more than 10 messages is detected, "consumerB" will experience a delay of 1000ms for each message it consumes.
The delay increases as the lag in "consumerA" increases, with thresholds set at 20 and 30 messages leading to delays of 5000ms and 10000ms respectively.

This setup allows for dynamic prioritization of consumers based on their processing lag,
allowing critical consumers to catch up when they are falling behind.
 */

using System;
using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string producerName = "producer";
const string sampleTopicA = "sample-topic-a";
const string sampleTopicB = "sample-topic-b";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        services =>
            services.AddKafkaFlowHostedService(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .CreateTopicIfNotExists(sampleTopicA, 6, 1)
                            .CreateTopicIfNotExists(sampleTopicB, 6, 1)
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic(sampleTopicA)
                                    .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(sampleTopicA)
                                    .WithGroupId("my-group")
                                    .WithName("consumerA")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonCoreSerializer>()
                                            .Add<ProcessMessagesMiddleware>()
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(sampleTopicB)
                                    .WithGroupId("my-group")
                                    .WithName("consumerB")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .Throttle(
                                                t => t
                                                    .OnConsumerLag("consumerA")
                                                    .WithInterval(TimeSpan.FromSeconds(1))
                                                    .AddAction(a => a.AboveThreshold(10).ApplyDelay(1_000))
                                                    .AddAction(a => a.AboveThreshold(20).ApplyDelay(5_000))
                                                    .AddAction(a => a.AboveThreshold(30).ApplyDelay(10_000)))
                                            .AddSerializer<JsonCoreSerializer>()
                                            .Add<ProcessMessagesMiddleware>()
                                    )
                            )
                    )
            ))
    .Build();

await host.StartAsync();

var producer = host.Services
    .GetRequiredService<IProducerAccessor>()
    .GetProducer(producerName);

Console.WriteLine("Type the number of messages to produce or 'exit' to quit:");

while (true)
{
    var input = Console.ReadLine();

    if (int.TryParse(input, out var count))
    {
        for (var i = 0; i < count; i++)
        {
            _ = producer.ProduceAsync(
                sampleTopicA,
                Guid.NewGuid().ToString(),
                new TestMessage($"Message: {Guid.NewGuid()}"));

            _ = producer.ProduceAsync(
                sampleTopicB,
                Guid.NewGuid().ToString(),
                new TestMessage($"Message: {Guid.NewGuid()}"));
        }
    }

    if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}

await host.StopAsync();

await Task.Delay(3000);

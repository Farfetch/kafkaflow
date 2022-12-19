using System;
using KafkaFlow;
using KafkaFlow.Sample.FlowControl;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

const string producerName = "producer-test";
const string consumerName = "consumer-test";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster =>
            {
                const string topicName = "flow-control-sample";
                cluster
                    .WithBrokers(new[] { "localhost:9092" })
                    .CreateTopicIfNotExists(topicName, 1, 1)
                    .AddProducer(
                        producerName,
                        producer => producer
                            .DefaultTopic(topicName)
                            .AddMiddlewares(m => m.AddSingleTypeSerializer<SampleMessage, NewtonsoftJsonSerializer>())
                    )
                    .AddConsumer(
                        consumer => consumer
                            .Topic(topicName)
                            .WithGroupId("print-console-handler")
                            .WithName(consumerName)
                            .WithInitialState(ConsumerInitialState.Stopped)
                            .WithBufferSize(100)
                            .WithWorkersCount(1)
                            .AddMiddlewares(
                                m => m
                                    .AddSingleTypeSerializer<SampleMessage, NewtonsoftJsonSerializer>()
                                    .Add<PrintConsoleMiddleware>()
                            )
                    );
            })
);

var provider = services.BuildServiceProvider();

var bus = provider.CreateKafkaBus();

await bus.StartAsync();

var producer = bus.Producers[producerName];
var consumer = bus.Consumers[consumerName];

while (true)
{
    Console.Write("Number of messages to produce, Start, Stop, Pause, Resume, or Exit:");
    var input = Console.ReadLine()?.ToLower();

    switch (input)
    {
        case var _ when int.TryParse(input, out var count):
            for (var i = 0; i < count; i++)
            {
                var message = new SampleMessage
                {
                    Id = Guid.NewGuid(),
                    Name = $"Name_{Guid.NewGuid()}"
                };

                producer.Produce(message.Id.ToString(), message);
            }

            break;

        case "pause":
            consumer.Pause(consumer.Assignment);
            Console.WriteLine("Consumer paused");

            break;

        case "resume":
            consumer.Resume(consumer.Assignment);
            Console.WriteLine("Consumer resumed");

            break;

        case "stop":
            await consumer.StopAsync();
            Console.WriteLine("Consumer stopped");

            break;

        case "start":
            await consumer.StartAsync();
            Console.WriteLine("Consumer started");

            break;

        case "exit":
            await bus.StopAsync();
            return;
    }
}
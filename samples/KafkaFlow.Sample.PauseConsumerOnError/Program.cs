using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Sample.PauseConsumerOnError;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

const string producerName = "StopOnErrorConsole";
const string topicName = "stop-on-error-topic";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .CreateTopicIfNotExists(topicName, 3, 1)
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )
                .AddConsumer(
                    consumer => consumer
                        .Topic(topicName)
                        .WithGroupId("console-handler")
                        .WithBufferSize(100)
                        .WithWorkersCount(3)
                        .AddMiddlewares(
                            middlewares =>
                                middlewares
                                    .Add<PauseConsumerOnExceptionMiddleware>()
                                    .AddSerializer<JsonCoreSerializer>()
                                    .AddTypedHandlers(h => h.AddHandler<MessageHandler>())
                        )
                )
        )
);

var provider = services.BuildServiceProvider();

var bus = provider.CreateKafkaBus();

await bus.StartAsync();

var producer = provider
    .GetRequiredService<IProducerAccessor>()
    .GetProducer(producerName);

while (true)
{
    Console.WriteLine(
        "Type the message to produce or 'exit' to quit (press enter to send an empty message and cause an exception):");
    var messageContent = Console.ReadLine();

    if ("exit".Equals(messageContent, StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }

    await producer.ProduceAsync(
        topicName,
        Guid.NewGuid().ToString(),
        new WelcomeMessage { Text = messageContent });
}
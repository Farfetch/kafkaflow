using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Producers;
using KafkaFlow.Sample.Auth;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

const string producerName = "PrintConsole";
const string topicName = "sample-authenticated-topic";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9093" })
                .CreateTopicIfNotExists(topicName, 6, 1)
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )
                .AddConsumer(
                    consumer => consumer
                        .Topic(topicName)
                        .WithGroupId("print-console-handler")
                        .WithBufferSize(100)
                        .WithWorkersCount(3)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<PrintConsoleHandler>())
                        )
                )
                .WithSecurityInformation(information =>
                {
                    information.SaslUsername = "admin";
                    information.SaslPassword = "admin-secret";
                    information.SaslMechanism = SaslMechanism.Plain;
                    information.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                })
        )
);

var provider = services.BuildServiceProvider();

var bus = provider.CreateKafkaBus();

await bus.StartAsync();

var producer = provider
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
            await producer.ProduceAsync(
                topicName,
                Guid.NewGuid().ToString(),
                new TestMessage { Text = $"Message: {Guid.NewGuid()}"});
        }
    }

    if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }
}

await Task.Delay(3000);

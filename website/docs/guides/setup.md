---
sidebar_position: 0
---

# Setup

## .NET Core 2.1 and later using Hosted Service

**Required Packages**
-  [KafkaFlow](https://www.nuget.org/packages/KafkaFlow/)
-  [KafkaFlow.Extensions.Hosting](https://www.nuget.org/packages/KafkaFlow.Extensions.Hosting/)
-  [KafkaFlow.Microsoft.DependencyInjection](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/)
-  [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)

**Optional Packages**
-  [KafkaFlow.Serializer](https://www.nuget.org/packages/KafkaFlow.Serializer/)
-  [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)
-  [KafkaFlow.TypedHandler](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)

```csharp
public static void Main(string[] args)
{
    Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddKafkaFlowHostedService(kafka => kafka
                .UseConsoleLog()
                .AddCluster(cluster => cluster
                    .WithBrokers(new[] { "localhost:9092" })
                    .AddConsumer(consumer => consumer
                        .Topic("sample-topic")
                        .WithGroupId("sample-group")
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .AddMiddlewares(middlewares => middlewares
                            .AddSerializer<NewtonsoftJsonMessageSerializer>()
                            .AddTypedHandlers(handlers => handlers
                                .AddHandler<SampleMessageHandler>())
                        )
                    )
                    .AddProducer("producer-name", producer => producer
                        .DefaultTopic("sample-topic")
                        .AddMiddlewares(middlewares => middlewares
                            .AddSerializer<NewtonsoftJsonMessageSerializer>()
                        )
                    )
                )
            );
        })
        .Build()
        .Run();
}
```

## Using Startup class

**Required Packages**
-  [KafkaFlow](https://www.nuget.org/packages/KafkaFlow/)
-  [KafkaFlow.Microsoft.DependencyInjection](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/)
-  [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)

**Optional Packages**
-  [KafkaFlow.Serializer](https://www.nuget.org/packages/KafkaFlow.Serializer/)
-  [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)
-  [KafkaFlow.TypedHandler](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddKafka(kafka => kafka
        .UseConsoleLog()
        .AddCluster(cluster => cluster
            .WithBrokers(new[] { "localhost:9092" })
            .AddConsumer(consumer => consumer
                .Topic("sample-topic")
                .WithGroupId("sample-group")
                .WithBufferSize(100)
                .WithWorkersCount(10)
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                    .AddTypedHandlers(handlers => handlers
                        .AddHandler<SampleMessageHandler>())
                )
            )
            .AddProducer("producer-name", producer => producer
                .DefaultTopic("sample-topic")
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                )
            )
        )
    );
}

public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env,
    IHostApplicationLifetime lifetime)
{
    var kafkaBus = app.ApplicationServices.CreateKafkaBus();

    lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));
}
```

## Unity or other DI containers

**Required Packages**
-  [KafkaFlow](https://www.nuget.org/packages/KafkaFlow/)
-  [KafkaFlow.Unity](https://www.nuget.org/packages/KafkaFlow.Unity/)
-  [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)

**Optional Packages**
-  [KafkaFlow.Serializer](https://www.nuget.org/packages/KafkaFlow.Serializer/)
-  [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)
-  [KafkaFlow.TypedHandler](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)

```csharp
var configurator = new KafkaFlowConfigurator(
    new UnityDependencyConfigurator(unityContainer),
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(cluster => cluster
            .WithBrokers(new[] { "localhost:9092" })
            .AddConsumer(consumer => consumer
                .Topic("sample-topic")
                .WithGroupId("sample-group")
                .WithBufferSize(100)
                .WithWorkersCount(10)
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                    .AddTypedHandlers(handlers => handlers
                        .AddHandler<SampleMessageHandler>())
                )
            )
            .AddProducer("producer-name", producer => producer
                .DefaultTopic("sample-topic")
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                )
            )
        )
);

var bus = configurator.CreateBus(new UnityDependencyResolver(unityContainer));

// Call when your app starts
await bus.StartAsync();

// Call when your app stops
await bus.StopAsync();
```
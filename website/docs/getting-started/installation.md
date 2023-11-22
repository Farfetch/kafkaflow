---
sidebar_position: 1
---


# Installation

KafkaFlow is a set of nuget packages.

## Prerequisites

 - One of the following .NET versions
   - .NET Core 2.1 or above.
   - .NET Framework 4.6.1 or above.


## Installing


Install KafkaFlow using NuGet package management.

Required Packages:
 - [KafkaFlow](https://www.nuget.org/packages/KafkaFlow/)
 - [KafkaFlow.Microsoft.DependencyInjection](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/)
 - [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)

You can quickly install them using .NET CLI 👇
```shell
dotnet add package KafkaFlow
dotnet add package KafkaFlow.Microsoft.DependencyInjection
dotnet add package KafkaFlow.LogHandler.Console
```

You can find a complete list of the available packages [here](packages.md).


## Setup

Types are in the `KafkaFlow` namespace.

```csharp
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
```

The host is configured using Dependency Injection. This is typically done once at application `Startup.cs` shown bellow, but you can find an example on how to do it with an [Hosted Service here](create-your-first-application.md).

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
                    .AddDeserializer<JsonCoreDeserializer>()
                    .AddTypedHandlers(handlers => handlers
                        .AddHandler<SampleMessageHandler>())
                )
            )
            .AddProducer("producer-name", producer => producer
                .DefaultTopic("sample-topic")
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<JsonCoreSerializer>()
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

Now you can create Message Handlers or access Producers and start exchanging events trouh Kafka.



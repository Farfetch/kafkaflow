---
sidebar_position: 0
---

# Configuration

In this section, we will introduce how configuration is done in KafkaFlow.

KafkaFlow is a highly configured framework. You can customize it through a Fluent Builder.

Using the builder, you can configure [Logging](../guides/logging.md), Clusters, Producers, Consumers and others.

There are a few options to configure KafkaFlow:
  - [Using a Hosted Service](#hosted-service)
  - [Using ASP.NET Core Startup](#aspnet-core-startup)
  - [Using Startup.cs](#startup-class)
  - [Using other DI Container (Unity or other)](#other-di-container)


## Using a Hosted Service {#hosted-service}

The Hosted Service model can be used as a hosting model on applications like Console apps.

Add the required package references:

```bash
dotnet add package KafkaFlow
dotnet add package KafkaFlow.Extensions.Hosting
dotnet add package KafkaFlow.Microsoft.DependencyInjection
dotnet add package Microsoft.Extensions.Hosting
```

Register KafkaFlow Hosted Service:

```csharp
using KafkaFlow;
using Microsoft.Extensions.Hosting;

public static class Program
{
    private static async Task Main(string[] args)
    {
        await Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddKafkaFlowHostedService(kafka => kafka
                    .AddCluster(cluster => cluster
                        .WithBrokers(new[] { "localhost:9092" })
                        ...
                    )
                );
            })
            .Build()
            .RunAsync();
    }
}
```


## Using ASP.NET Core Startup {#aspnet-core-startup}

After .NET 6 the `Startup.cs` class is not required.

Add the required package references:

```bash
dotnet add package KafkaFlow
dotnet add package KafkaFlow.Microsoft.DependencyInjection
```

To configure KafkaFlow, use the builder to register KafkaFlow dependencies and start the Kafka Bus before the application run. 

```csharp
using KafkaFlow;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        ...
    )
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Run();
```


## Using Startup.cs {#startup-class}

Add the required package references:

```bash
dotnet add package KafkaFlow
dotnet add package KafkaFlow.Microsoft.DependencyInjection
```

To configure KafkaFlow, use the `ConfigureServices` method to register KafkaFlow dependencies, and on the `Configure` method register an event to start the Kafka Bus on application start.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddKafka(kafka => kafka
        .UseConsoleLog()
        .AddCluster(cluster => cluster
            .WithBrokers(new[] { "localhost:9092" })
            ...
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


## Using other DI Container (Unity or other) {#other-di-container}

:::info

If you want to use a DI container other than Unity, check [how to implement](dependency-injection.md) it first.

:::

Add the required package references:

```bash
dotnet add package KafkaFlow
dotnet add package KafkaFlow.Unity
```

Use `KafkaFlowConfigurator` to specify the desired Dependency Injection container:

```csharp
using KafkaFlow.Configuration;
using KafkaFlow.Unity;
using Unity;


static class Program
{
    public static async Task Main(string[] args)
    {
        var container = new UnityContainer();

        var configurator = new KafkaFlowConfigurator(
            new UnityDependencyConfigurator(container),
            kafka => kafka
                .AddCluster(cluster => cluster
                    .WithBrokers(new[] { "localhost:9092" })
                    ...
                )
        );

        var bus = configurator.CreateBus(new UnityDependencyResolver(container));

        // Call when your app starts
        await bus.StartAsync();

        // Call when your app stops
        await bus.StopAsync();
    }
}
```
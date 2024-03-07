---
sidebar_position: 3
---

# Dashboard

In this section, we will learn how to configure the Dashboard.

KafkaFlow provides a Dashboard where you can visualize information related to your consumers and use all operations available on [KafkaFlow.Admin.WebApi](web-api). 

:::caution
It is important to note that the Dashboard runs, shows information, and manages the consumers on all application instances. This means that if you have 10 machines running your application, the Dashboard will run in every instance. **Any operation will affect the consumer in all the machines**.
:::

## Adding the Dashboard

Install the following packages:
* [KafkaFlow.Admin](https://www.nuget.org/packages/KafkaFlow.Admin/)
* [KafkaFlow.Admin.WebApi](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)
* [KafkaFlow.Admin.Dashboard](https://www.nuget.org/packages/KafkaFlow.Admin.Dashboard/)

```bash
dotnet add package KafkaFlow.Microsoft.DependencyInjection
dotnet add package KafkaFlow.Admin
dotnet add package KafkaFlow.Admin.WebApi
dotnet add package KafkaFlow.Admin.Dashboard
```


You can configure the Dashboard during the [configuration](../configuration), as shown in the following example (*built using .NET 6*).

```csharp
using KafkaFlow;
using KafkaFlow.Admin.Dashboard;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddKafka(kafka => kafka
        .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddConsumer(consumer => consumer
                    .Topic("mytopic")
                    .WithGroupId("g1")
                    .WithWorkersCount(1)
                    .WithBufferSize(10)
                )
                .EnableAdminMessages("kafka-flow.admin")
                .EnableTelemetry("kafka-flow.admin") // you can use the same topic used in EnableAdminMessages, if need it
        ))
    .AddControllers();

var app = builder.Build();

app.MapControllers();
app.UseKafkaFlowDashboard();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await app.RunAsync();
```

The dashboard UI will be available at `/kafkaflow` and is refreshed every 5 seconds with telemetry data available at the endpoint `/kafkaflow/consumers/telemetry`.

![image](https://user-images.githubusercontent.com/233064/124478023-1d773680-dd7b-11eb-89e4-41a1f4f36a6f.png)

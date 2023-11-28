---
sidebar_position: 2
sidebar_label: Web API
---

# Administration Web API

In this section, we will learn how to configure the Administration Web API.

You can configure KafkaFlow to expose an API for administrative operations.

The API can be used either manually (for example, you can install and use [Swagger](#swagger)) or by any other application using the available endpoints. 

Furthermore, you can execute the operations publishing commands directly to the KafkaFlow admin topics using the class `AdminProducer`.


:::caution
It is important to note that these operations will be executed on all application instances. If you have 10 machines running your application, **one single POST** to pause a specific consumer will pause it in **all** the machines.
:::


## Adding the Admin API

Install the following packages:
* [KafkaFlow.Admin](https://www.nuget.org/packages/KafkaFlow.Admin/)
* [KafkaFlow.Admin.WebApi](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)

```bash
dotnet add package KafkaFlow.Microsoft.DependencyInjection
dotnet add package KafkaFlow.Admin
dotnet add package KafkaFlow.Admin.WebApi
```

You can configure the API during the [configuration](../configuration) by enabling Admin Messages on the Cluster configuration, as shown in the following example (*built using .NET 6*).

```csharp
using KafkaFlow;

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
            .EnableAdminMessages(
                "kafka-flow.admin" // the admin topic
            )
        ))
    .AddControllers();

var app = builder.Build();

app.MapControllers();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await app.RunAsync();
```

The API will be accessed on `/kafka-flow/`.

## Adding Swagger to Admin API  {#swagger}

It's possible to generate a [Swagger](https://swagger.io/) interface for documentation and as UI to explore and test operations.

:::info
This is not a KafkaFlow feature. This guide works as a starting guide on how to configure it using [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
For advanced information on it, go to Swashbuckle documentation.
:::

Install Swashbuckle package.

```bash
dotnet add package Swashbuckle.AspNetCore
```

Register the Swagger Generator on the services configuration.

```csharp
using Microsoft.OpenApi.Models;

builder.Services
    .AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc(
                "kafka-flow",
                new OpenApiInfo
                {
                    Title = "KafkaFlow Admin",
                    Version = "kafka-flow",
                });
        });
```

Expose the generated documentation.

```bash
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/kafka-flow/swagger.json", "KafkaFlow Admin");
});
```

Swagger UI will be accessed on `/swagger/`.


## API Endpoints

![admin-swagger-1](https://user-images.githubusercontent.com/233064/98698756-5129ca00-236e-11eb-9a70-e0f997050cd6.jpg)

### Consumers

#### Start
Start a consumer based on his name.

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.StartConsumerAsync(consumerName);
```

#### Stop
Stop a consumer based on his name.

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.StopConsumerAsync(consumerName);
```

#### Pause
Pause all Kafka consumers based on their name and groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/pause`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.PauseConsumerAsync(consumerName, new []{ topicName });
```

#### Resume
Resume all Kafka consumers based on their name and groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/resume`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.ResumeConsumerAsync(consumerName, new []{ topicName });
```

#### Restart
Restart all Kafka consumers based on their. This operation will not change any offsets, it's a simple restart. The internal Confluent Consumer will be recreated. This operation causes a partition rebalanced between the consumers.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/restart`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.RestartConsumerAsync(consumerName);
```

#### Reset Offsets
Reset the offset of all topics listening by the Kafka consumers with the name and groupId informed. To achieve this, KafkaFlow needs to stop the consumers, search for the lowest offset value in each topic/partition, commit these offsets, and restart the consumers. This operation causes a rebalance between the consumers. ** All topic messages will be reprocessed **

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/reset-offsets`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.ResetOffsetsAsync(consumerName, new []{ topicName });
```

#### Rewind Offsets
Rewind the offset of all topics listening by a Kafka consumer with its name and groupId. To achieve this, KafkaFlow needs to stop the consumers, search for the first offset before the DateTime informed, commit the new offsets, and restart the consumers. This operation causes a rebalance between the consumers.

Endpoint

```
POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/rewind-offsets-to-date

BODY
{
  "date": "2000-11-09T17:52:54.547Z"
}
 
```

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.RewindOffsetsAsync(consumerName, DateTime.Today.AddDays(-1), new []{ topicName });
```
#### Change the Number of Workers​
Change the numbers of workers (degree of parallelism) for the KafkaFlow consumer with the name and groupId informed. This operation causes a rebalance between the consumers.

Endpoint

```
POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/change-worker-count

BODY
{
  "workerCount": 0
}
 
```

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.ChangeWorkersCountAsync(consumerName, 100);
```

### Consumer Group

#### Pause
Pause all Kafka consumers based on their groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/pause`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.PauseConsumerGroupAsync(groupId, new []{ topicName });
```

#### Resume
Resume all Kafka consumers based on groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/resume`

Command
```csharp
var consumerAdmin = provider.GetService<IConsumerAdmin>();
await consumerAdmin.ResumeConsumerGroupAsync(groupId, new []{ topicName });
```

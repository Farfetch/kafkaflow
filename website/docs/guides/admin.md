---
sidebar_position: 8
---

# Admin

KafkaFlow provides a [Web API](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/) where you can get details about the KafkaFlow consumers and perform several administration operations over them. This can be manually (for example, you can install and use [Swagger](https://swagger.io)) or by any other applications using the available endpoints. 
Furthermore, you can execute the operations publishing commands directly to the KafkaFlow admin topics using the class `AdminProducer`.

It is important to note that these operations will be executed on all application instances, that means if you have 10 machines running your application, **one single POST** to pause a specific consumer will pause the consumer in **all** the machines.

## Configuring
Install the following packages:
* [KafkaFlow.Admin](https://www.nuget.org/packages/KafkaFlow.Admin/)
* [KafkaFlow.Admin.WebApi](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)


```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .EnableAdminMessages(
             "kafka-flow.admin"  // the admin topic
        )
    )
);
```
## Endpoints

![admin-swagger-1](https://user-images.githubusercontent.com/233064/98698756-5129ca00-236e-11eb-9a70-e0f997050cd6.jpg)

## Consumers

### Pause
Pause all Kafka consumers based on its name and groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/pause`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new PauseConsumerByName { ConsumerName = consumerName });
```

### Resume
Resume all Kafka consumers based on its name and groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/resume`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new ResumeConsumerByName{ ConsumerName = consumerName });
```

### Restart
Restart all Kafka consumers based on its name and groupId. This operation will not change any offsets, it's a simple restart. The internal Confluent Consumer will be recreated. This operation causes a partition rebalanced between the consumers.

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/restart`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new RestartConsumerByName{ ConsumerName = "consumerName" });
```

### Reset Offsets
Reset the offset of all topics listening by the Kafka consumers with the name and groupId informed. To achieve this, KafkaFlow needs to stop the consumers, search for the lowest offset value in each topic/partition, commit these offsets, and restart the consumers. This operation causes a rebalance between the consumers. ** All topic messages will be reprocessed **

Endpoint

`POST /kafka-flow/groups/{groupId}/consumers/{consumerName}/reset-offsets`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new ResetConsumerOffset{ ConsumerName = "consumerName" });
```

### Rewind Offsets
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
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new RewindConsumerOffsetToDateTime
{ 
   ConsumerName = "consumerName",
   DateTime = DateTime.Now
});
```
### Change Number of Worker 
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
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new ChangeConsumerWorkerCount
{ 
   ConsumerName = "consumerName",
   WorkerCount = 100
});
```

## Consumer Group

### Pause
Pause all Kafka consumers based on its groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/pause`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new PauseConsumersByGroup{ GroupId = "groupId"});
```

### Resume
Resume all Kafka consumers based on groupId.

Endpoint

`POST /kafka-flow/groups/{groupId}/resume`

Command
```csharp
var adminProducer = provider.GetService<IAdminProducer>();
adminProducer.ProduceAsync(new ResumeConsumersByGroup{ GroupId = "groupId"});



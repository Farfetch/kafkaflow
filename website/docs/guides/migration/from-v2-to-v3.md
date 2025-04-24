---
sidebar_position: 1
---

# From v2 to v3

KafkaFlow version 3 brings several significant changes and improvements. This guide will help you navigate through the migration process from version 2 to version 3.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Update package references](#update-package-references)
- [Breaking Changes](#breaking-changes)
  - [1. Update to .NET 6 with Admin Packages](#1-update-to-net-6-with-admin-packages)
  - [2. UI Dashboard URL Change](#2-ui-dashboard-url-change)
  - [3. Removed Packages and Core Changes](#3-removed-packages-and-core-changes)
    - [3.1 Removed Packages](#31-removed-packages)
    - [3.2 Serializer and Compressor Interface Segregation](#32-serializer-and-compressor-interface-segregation)
    - [3.3 Consumer Batching Configuration](#33-consumer-batching-configuration)
    - [3.4. Async Support for Message Type Resolvers and Schema Registry Resolvers](#34-async-support-for-message-type-resolvers-and-schema-registry-resolvers)
- [New Features](#new-features)
  - [1. Dynamic Workers Calculation](#1-dynamic-workers-calculation)
    - [1.1 Dynamic Worker Pool Scaling Configuration](#11-dynamic-worker-pool-scaling-configuration)
    - [1.2 Improved Mechanism for Signalling Message Processing Completion](#12-improved-mechanism-for-signalling-message-processing-completion)
    - [1.3 Expose Worker Events to Client Applications](#13-expose-worker-events-to-client-applications)
    - [1.4 Improve Dependency Injection Scope Management](#14-improve-dependency-injection-scope-management)
  - [2. Improved Worker Distribution Strategy](#2-improved-worker-distribution-strategy)
- [Conclusion](#conclusion)


## Prerequisites

As .NET Core 3.1 has reached the end of support in its lifecycle, we have updated references to Core Packages (`Microsoft.*` and `System.*`) to version 6.

While the KafkaFlow core and most of the extension packages are still targeting `netstandard2.0` which supports a range of runtimes, we recommend with this v3 update to target at least .NET 6 in applications using KafkaFlow.

## Update package references

To update to KafkaFlow v3, change the `Version` related to KafkaFlow packages to the latest v3 available in each project referencing KafkaFlow packages.

```
<ItemGroup>
-   <PackageReference Include="KafkaFlow" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Abstractions" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Admin" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Admin.Dashboard" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Admin.WebApi" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.BatchConsume" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Compressor" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Compressor.Gzip" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Extensions.Hosting" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.LogHandler.Console" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.LogHandler.Microsoft" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Microsoft.DependencyInjection" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.OpenTelemetry" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.SchemaRegistry" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.JsonCore" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.NewtonsoftJson" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.ProtobufNet" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentJson" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentProtobuf" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.TypedHandler" Version="2.5.0" />
-   <PackageReference Include="KafkaFlow.Unity" Version="2.5.0" />

+   <PackageReference Include="KafkaFlow" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Abstractions" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Admin" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Admin.Dashboard" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Admin.WebApi" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Compressor.Gzip" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Extensions.Hosting" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.LogHandler.Console" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.LogHandler.Microsoft" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Microsoft.DependencyInjection" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.OpenTelemetry" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.SchemaRegistry" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.JsonCore" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.NewtonsoftJson" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.ProtobufNet" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentJson" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentProtobuf" Version="3.0.0" />
+   <PackageReference Include="KafkaFlow.Unity" Version="3.0.0" />
</ItemGroup>
```

## Breaking Changes

The update to v3 introduces some breaking changes. Please consider them when updating KafkaFlow.

### 1. Update to .NET 6 with Admin Packages

> Related Issues: [#298](https://github.com/Farfetch/kafkaflow/issues/298) [#322](https://github.com/Farfetch/kafkaflow/issues/322)

The target frameworks for ASP.NET Admin projects were updated from `netcoreapp3.1` to `.net6.0`.

If you are using the Admin packages (`KafkaFlow.Admin.Dashboard` and `KafkaFlow.Admin.WebApi `) you will need to target the .NET 6 runtime.

### 2. UI Dashboard URL Change

> Related Issues: [#303](https://github.com/Farfetch/kafkaflow/issues/303)

To be in conformity with KafkaFlow naming conventions, the UI Dashboard URL has changed from `/kafka-flow` to `/kafkaflow`. Update any bookmarks or references accordingly.

### 3. Removed Packages and Core Changes

#### 3.1 Removed Packages

> Related Issues: [#301](https://github.com/Farfetch/kafkaflow/issues/301)

Because some KafkaFlow extension packages contained elements that belong to the core of KafkaFlow, these packages were removed and moved to the core library.
Most of these packages contained only interfaces or core functionalities that were moved to the core library while the concrete implementations continue to exist as separate packages to keep KafkaFlow's modular design.

Here is the list of KafkaFlow packages that no longer exist in v3. In case you are referencing any of these packages, please remove those references since their references were moved to the core KafkaFlow package:

- KafkaFlow.TypedHandler
- KafkaFlow.Compressor
- KafkaFlow.Serializer
- KafkaFlow.BatchConsume

#### 3.2 Serializer and Compressor Interface Segregation

> Related Issues: [#432](https://github.com/Farfetch/kafkaflow/issues/432), [#433](https://github.com/Farfetch/kafkaflow/issues/433)

Before KafkaFlow v3, references were used on the consumer side, for example, `AddSerializer()` or `.AddCompressor()`. While this convention was used for simplicity's sake to keep the same nomenclature as in the producer counterpart, the nomenclature can be error-prone since on the consumer side the behavior when using  `AddSerializer()` is adding a middleware for deserializing the message.

To fix this, on KafkaFlow v3, the `IDeserializer` and `IDecompressor` were created to segregate from the `ISerializer` and `ICompressor` interfaces.

In case you are configuring the consumer to deserialize and/or decompress a message similar to this configuration:

```csharp
.AddConsumer(
    consumerBuilder => consumerBuilder
        .Topic("test-topic")
        .WithGroupId("group1")
        .AddMiddlewares(
            middlewares => middlewares
                .AddCompressor<GzipMessageCompressor>()
                .AddSerializer<JsonCoreSerializer>()
        )
)
```

The following changes need to be made when updating to KafkaFlow v3.

```csharp
.AddConsumer(
    consumerBuilder => consumerBuilder
        .Topic("test-topic")
        .WithGroupId("group1")
        .AddMiddlewares(
            middlewares => middlewares
                .AddDecompressor<GzipMessageDecompressor>()
                .AddDeserializer<JsonCoreDeserializer>()
        )
)
```

Not only the `.AddSerializer()` and `.AddCompressor()` were renamed to `.AddDeserializer()` and `.AddDecompressor()`, but also the concrete implementations like `JsonCoreSerializer` and `GzipMessageCompressor` were renamed to `JsonCoreDeserializer` and `GzipMessageDecompressor`. Please be aware that this change is only on the **consumer side**.

Having this segregation contributes to a more consistent name scheme together with the middleware behavior. Nevertheless, other changes related to this topic were made, here is a complete list of changes related to the segregation of `ISerializer` and `ICompressor` interfaces (**consumer configuration** only):

Related to Deserialization:

- `IDeserializer` interface created.
- `.AddSerializer()` renamed to `.AddDeserializer()`.
- Created `JsonCoreDeserializer `, `ProtobufNetDeserializer`, `ConfluentProtobufDeserializer`, `NewtonsoftJsonDeserializer`, `ConfluentJsonDeserializer` and `ConfluentAvroDeserializer` that implements the `.DeserializeAsync()` method.
- `.AddSingleTypeDeserializer()` renamed to `.AddSingleTypeDeserializer()`
- `.AddSingleTypeSerializer()` renamed to `.AddSchemaRegistryAvroDeserializer()`

Related to Decompression:

- `IDecompressor` interface created.
- `.AddCompressor()` renamed to `.AddDecompressor()`.
- Created `GzipMessageDecompressor` that implements the `.Decompress()` method.

#### 3.3 Consumer Batching Configuration

Consumer batching configuration renamed from `.BatchConsume()` to `.AddBatching()`. Update your configuration accordingly.

#### 3.4. Async Support for Message Type Resolvers and Schema Registry Resolvers

> Related Issues: [#302](https://github.com/Farfetch/kafkaflow/issues/302), [#305](https://github.com/Farfetch/kafkaflow/issues/305)

In both `IMessageTypeResolvers` and `ISchemaRegistryTypeNameResolver` the interface methods only supported sync calls before KafkaFlow v3.
To accommodate some use cases where calls to these methods can be parallelized, the methods defined in this interface were refactored to support the Async pattern.

If your application has a concrete implementation of these interfaces, please make sure to update your code accordingly by using the async/await pattern. Here is the list of changes related to this subject:

- `Resolve()` renamed to `ResolveAsync()` in `ISchemaRegistryTypeNameResolver` and now returns a `Task<string>`.
- `OnConsume()` renamed to `OnConsumeAsync()` in `IMessageTypeResolver` and returns a `ValueTask<Type>`.
- `OnProduce()` renamed to `OnProduceAsync()` in `IMessageTypeResolver` and returns a `ValueTask`.

## New Features

Additionally, with the update to v3, some quality-of-life features were added to KafkaFlow core, mainly related to how workers are configured.

### 1. Dynamic Workers Calculation

One important feature added to KafkaFlow v3 was the ability to calculate how many workers are handling the message consumption dynamically. Please check the feature documentation [here](https://farfetch.github.io/kafkaflow/docs/guides/consumers/dynamic-workers-configuration).

In load scenarios where the consumer needs to ramp up message consumption to reduce the consumer lag, increasing the number of workers can be a valid strategy in this scenario.

Before, the number of workers was determined statically by configuration at the time of application startup. With KafkaFlow v3, a new method was introduced that supports calculating the number of available workers periodically. This allows the application to adapt constantly and also being able to scale the number of workers during the application's lifetime.

To accommodate this requirement multiple changes were made to the core of KafkaFlow, next we will go through some of these changes.

### 1.1 Dynamic Worker Pool Scaling Configuration

> Related Issues: [#429](https://github.com/Farfetch/kafkaflow/issues/429)

Two new overrides were added to the consumer configuration. Nevertheless, the previous method of statically determining the worker count is still available.

The new overrides added were:

- `WithWorkersCount(Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator)`
- `WithWorkersCount(Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator, TimeSpan evaluationInterval)`

The difference between both overrides is that the first works with a predefined frequency of 5 minutes between calls to the delegate that returns the number of workers. While in the second override that frequency can be user-defined.

Additionally, the delegate receives a `WorkersCountContext` that sends some consumer-related context to the callback like the Consumer Name, the Consumer Group Key and the Consumer Partitions Assignment. Also, the `IDependencyResolver` is made available in case some dependencies need to be resolved to determine the worker count.

This means that a consumer worker count can be configured like this:

```csharp
.AddConsumer(
    consumerBuilder => consumerBuilder
        .Topic("test-topic")
        .WithGroupId("group1")
        .WithWorkersCount((context, resolver) => {
          // Use whatever logic to determine the worker count
          return GetWorkerCount(context, resolver);
        }, TimeSpan.FromMinutes(15))
)
```

### 1.2 Improved Mechanism for Signalling Message Processing Completion

> Related Issues: [#426](https://github.com/Farfetch/kafkaflow/issues/426)

For the KafkaFlow engine to adapt to the dynamic change of workers, the way the messages are signaled as completed also needed to be updated because the workers can only be scaled after all the messages in the worker's buffers are signaled as completed.

To achieve this, a new method in the message context was provided to signal the message completion `.Complete()`. Additionally, a new property called `AutoMessageCompletion` was added to signal if KafkaFlow should signal the message as completed automatically which is the default behavior. This property is useful for scenarios where we don't want to complete the message right away, for example in Batch Consume scenarios.

- Use `WithManualMessageCompletion()` instead of `WithManualStoreOffsets()`.
- Use `context.ConsumerContext.Complete()` instead of `context.ConsumerContext.StoreOffset()`.

### 1.3 Expose Worker Events to Client Applications

> Related Issues: [#427](https://github.com/Farfetch/kafkaflow/issues/427)

KafkaFlow now exposes some events related to its internal functionality. The events that are being published are the following:

- MessageProduceStarted
- MessageProduceCompleted
- MessageProduceError
- MessageConsumeStarted
- MessageConsumeCompleted
- MessageConsumeError
- WorkerStopping
- WorkerStopped
- WorkerProcessingEnded

While worker-related events are available through the `IWorker` reference, the message-related events can be subscribed using the `.SubscribeGlobalEvents()` configuration method.

```csharp
services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .SubscribeGlobalEvents(hub =>
        {
            hub.MessageConsumeStarted.Subscribe(eventContext => Console.WriteLine("Message Consume Started"));

            hub.MessageConsumeError.Subscribe(eventContext => Console.WriteLine("Message Consume Error"));

            hub.MessageConsumeCompleted.Subscribe(eventContext => Console.WriteLine("Message Consume Completed"));

            hub.MessageProduceStarted.Subscribe(eventContext => Console.WriteLine("Message Produce Started"));

            hub.MessageProduceError.Subscribe(eventContext => Console.WriteLine("Message Produce Error"));

            hub.MessageProduceCompleted.Subscribe(eventContext => Console.WriteLine("Message Produce Completed"));
        })
);
```

### 1.4 Improve Dependency Injection Scope Management

> Related Issues: [#428](https://github.com/Farfetch/kafkaflow/issues/428)

Some improvements were made to the way the dependency scopes are managed to provide more fine-grained control over dependency lifecycles. Here are some changes that were made in this context:

- `IMessageContext` now exposes a property `DependencyResolver` which is intrinsically tied to the scope of a single processed message.
- `IConsumerContext` introduces two properties: `ConsumerDependencyResolver` and `WorkerDependencyResolver`. The former will resolve dependencies tied to the lifecycle of a single consumer, whereas the latter caters to a single worker's lifecycle.

## 2. Improved Worker Distribution Strategy

> Related Issues: [#440](https://github.com/Farfetch/kafkaflow/issues/440)

KafkaFlow v2 already provided a method to define the worker to handle a message when consuming using the `IDistributionStrategy` interface. However, the only context provided by this method was the partition key of the message, which sometimes can be insufficient to determine how to distribute messages across workers in more complex scenarios.

With KafkaFlow v3 this interface got renamed to `IWorkerDistributionStrategy` and now the method `.GetWorkerAsync()` receives a `WorkerDistributionContext` structure. Properties like message topic and partition are now part of the context which provides more flexibility when choosing the worker to handle a message.

## Conclusion

Please ensure you review and adapt your codebase according to these changes. If you encounter any issues or need assistance, feel free to [reach out](https://github.com/Farfetch/kafkaflow#get-in-touch) to the KafkaFlow community. Thank you for using KafkaFlow!

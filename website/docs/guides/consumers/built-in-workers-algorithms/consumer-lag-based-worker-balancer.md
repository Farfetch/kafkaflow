---
sidebar_position: 1
---

# Consumer Lag-Based Worker Balancer

The `WithConsumerLagWorkerBalancer` method in KafkaFlow is a powerful feature that allows you to dynamically calculate the number of workers for each application instance based on the consumer's lag. This feature is designed to optimize message processing, especially in scenarios where some application instances have more partitions and naturally need to deal with higher message throughput. By adjusting the number of workers based on message lag, this feature helps ensure efficient message processing and load balancing across all application instances.

In this documentation, you'll find an overview of how this feature works, a use case example, and step-by-step instructions on how to configure it.

:::info
This method replaces the call of `WithWorkersCount` in the consumer's setup.
:::

## Use Case Example

### Balancing Message Lag Across Application Instances

Consider a scenario where you have a Kafka consumer application deployed in a distributed environment. In this environment, some application instances may have more partitions assigned to them, resulting in a higher message throughput potential. However, variations in message lag across partitions can lead to uneven workloads.

Here's how the `WithConsumerLagWorkerBalancer` feature can help:

- **Total Workers**: You specify the `totalWorkers`, which represents the total number of workers to be distributed across all application instances.

- **Minimum and Maximum Instance Workers**: You set the `minInstanceWorkers` and `maxInstanceWorkers` parameters, defining the minimum and maximum number of workers allowed for each application instance.

- **Evaluation Interval**: You define an `evaluationInterval` that determines how often the number of workers should be recalculated based on the consumer's lag.

With this configuration, the `WithConsumerLagWorkerBalancer` feature dynamically adjusts the number of worker threads for each application instance based on the lag in the Kafka topic. Application instances with partitions experiencing higher lag will have more workers allocated to help balance the message lag across all instances.

### Benefits in Elastic Infrastructure

This feature is particularly valuable in elastic infrastructure environments like Kubernetes, where you need to manage the total number of workers across all application instances to prevent overloading dependencies. By dynamically adjusting worker counts, the feature ensures that each instance scales its resources efficiently, improving overall application performance and resource utilization.

## How to Configure

Configuring Consumer Lag-Based Worker Balancer is straightforward with the fluent interface provided by KafkaFlow. Here's a simple example:

```csharp
.AddConsumer(
    consumer => consumer
        ...
        .WithConsumerLagWorkerBalancer(
            50, // The total number of workers to be distributed across all application instances.
            3,  // The minimum number of workers for each application instance.
            20) // The maximum number of workers for each application instance.
        ...
        )
)
```

With this configuration, KafkaFlow will dynamically adjust the number of worker threads for each application instance, ensuring efficient message processing while considering the lag in the Kafka topic. This feature provides a powerful way to optimize resource allocation in your Kafka-based applications, making them more adaptive to varying message loads and distribution across partitions.

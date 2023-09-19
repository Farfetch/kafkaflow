---
sidebar_position: 3
---

# Dynamic Worker Configuration

In this section, we will learn how Dynamic Worker Configuration works and how to configure it. In version 3 of KafkaFlow, we have introduced a new feature that allows you to dynamically configure the number of workers for a specific consumer based on a custom algorithm. This feature enables greater flexibility in managing worker threads, as each application instance can have a different number of workers, depending on the algorithm you define.

This documentation page explains how to use and configure this feature effectively.

## Use Case Example

Imagine a scenario where your application's message load varies throughout the day. During peak hours, you want to allocate more worker threads to process messages quickly, and during off-peak hours, you want to reduce the number of worker threads to save resources. This dynamic adjustment can be achieved using the custom dynamic worker configuration feature.

## How to Configure

Configuring Dynamic Worker Configuration is straightforward with the fluent interface provided by KafkaFlow. Here's a simple example:

```csharp
.AddConsumer(
    consumer => consumer
        ...
        .WithWorkersCount(
            (context, resolver) =>
            {
                // Implement a custom logic to calculate the number of workers
                if (IsPeakHour(DateTime.UtcNow))
                {
                    return Task.FromResult(10); // High worker count during peak hours
                }
                else
                {
                    return Task.FromResult(2); // Lower worker count during off-peak hours
                }
            }, 
            TimeSpan.FromMinutes(15)); // Evaluate the worker count every 15 minutes
        ...
        )
)
```

In this example, the number of worker threads is adjusted dynamically based on whether it's a peak hour or off-peak hour. You can implement your custom logic in the `WithWorkersCount`` method to suit your application's specific requirements.

That's it! Your KafkaFlow consumer will now dynamically adjust the number of worker threads based on your custom logic and the specified evaluation interval.

This feature provides a powerful way to optimize resource utilization and throughput in your Kafka-based applications.

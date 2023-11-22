---
sidebar_position: 6
---

# Consumer Throttling

In this section, we will learn how Consumer Throttling works and how to configure it. Consumer Throttling is a feature in KafkaFlow that allows you to dynamically manage the rate at which your application consumes messages from Kafka topics. It provides a mechanism to slow down or pause the consumption of messages based on specified metrics. This can be particularly useful in scenarios where you want to prioritize certain types of messages over others, or when there is a need to control the rate of processing during high-load periods or when dealing with backlogs.

This functionality is highly extensible, allowing for the use of custom metrics and actions by implementing the `IConsumerThrottlingMetric` and `IConsumerThrottlingAction` interfaces respectively. Although KafkaFlow comes with built-in implementations like Consumer Lag for metrics and Delay for actions, you can create your own based on your specific needs.

## Use Case Example

One example of how Consumer Throttling can be utilized effectively is when you want to segregate single and bulk actions into different consumers and topics. For instance, you may want to prioritize the processing of single actions and slow down the processing of bulk actions. This can be achieved by monitoring the consumer lag of the consumer responsible for single actions and applying throttling to the consumer handling the bulk actions based on this metric.

## How to Configure Consumer Throttling

Configuring Consumer Throttling is straightforward with the fluent interface provided by KafkaFlow. Here's a simple example:

```csharp
.AddConsumer(
    consumer => consumer
        .Topic("bulk-topic")
        .WithName("bulkConsumer")
        .AddMiddlewares(
            middlewares => middlewares
                .ThrottleConsumer(
                    t => t
                        .ByOtherConsumersLag("singleConsumer")
                        .WithInterval(TimeSpan.FromSeconds(5))
                        .AddAction(a => a.AboveThreshold(10).ApplyDelay(100))
                        .AddAction(a => a.AboveThreshold(100).ApplyDelay(1_000))
                        .AddAction(a => a.AboveThreshold(1_000).ApplyDelay(10_000)))
                .AddDeserializer<JsonCoreDeserializer>()
        )
)
```

## Consumer Throttling Methods

Here's a brief overview of the methods used to configure Consumer Throttling:

- `ThrottleConsumer`: This method enables the Throttling feature for the consumer.

- `ByOtherConsumersLag`: This extension method of `AddMetric` sets which consumers' lag should be monitored. The throttling will be applied based on these consumers' lag.

- `WithInterval`: This method specifies the interval at which the metrics will be checked and the throttling actions applied.

- `AddAction`: This method allows you to define actions that will be taken when certain metric thresholds are met. Actions can include applying a delay or pausing the consumer.

- `AboveThreshold`: This method sets the metric threshold at which the action will be applied.

- `ApplyDelay`: This extension method of `Apply` sets a delay to the consumer when the specified threshold is met.

These extension methods, `ByOtherConsumersLag` and `ApplyDelay`, are convenient ways to set up commonly used metrics and actions. But as stated earlier, KafkaFlow allows you to implement your own custom metrics and actions via the `IConsumerThrottlingMetric` and `IConsumerThrottlingAction` interfaces respectively.

In summary, Consumer Throttling is a powerful tool for managing message consumption in KafkaFlow. It brings flexibility and control to your message processing workflows and allows for effective prioritization and rate control. Whether you're using built-in metrics and actions or implementing your own, you can fine-tune your consumers to perform optimally under various conditions.

:::tip
You can find a sample on batch processing [here](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.ConsumerThrottling).
:::

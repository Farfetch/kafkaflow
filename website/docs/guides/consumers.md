---
sidebar_position: 4
---

# Consumers

Here is where KafkaFlow shines. Using KafkaFlow you have full control over how to consume the messages. Every consumer has its own [Workers](#workers) and [Middlewares](#middlewares) configuration. You can have multiple consumers consuming the same topic with different consumer groups or one consumer with multiple topics.

## Message Flow
![Message Flow](https://user-images.githubusercontent.com/233064/98690729-24bd8000-2365-11eb-8bd0-19e6aeeaebda.jpg)

Every KafkaFlow consumer is composed by a group of components: [Kafka Consumer](#kafka-consumer), [Consumer Worker Pool](#consumer-worker-pool), [Distribution Strategy](#distribution-strategy), [Workers](#workers), [Middlewares](#middlewares), and [Offset Manager](#offset-manager).

### Kafka Consumer

It’s where the Confluent Client runs. It has a background task that fetches the messages from any topics/partitions assigned for that consumer and delivers them to the [Consumer Worker Pool](#consumer-worker-pool). If the Confluent Consumer stops working for any reason (if a fatal exception occurs), the consumer will be recreated.

### Consumer Worker Pool

It orchestrates the Workers creation and destruction when the application starts, stops, and when partitions are assigned or revoked. It receives the message from [Kafka Consumer](#kafka-consumer) and uses the [Distribution Strategy](#distribution-strategy) to choose a [Worker](#workers) to enqueue the messages.

### Distribution Strategy

It’s an algorithm to choose a [Worker](#workers) to process the message. The Framework has two: **BytesSum** and **FreeWorker**. The **BytesSum** maintains the message order with some performance and resource penalties, **it is the default strategy**. The **FreeWorker** is faster but the message order is lost. A custom strategy can be implemented using the `IDistibutionStrategy` interface. You can configure the consumer's strategy during the configuration process with the method `WithWorkDistributionStrategy`.

### Workers

Workers are responsible for processing messages when consuming. You define how many workers a consumer will have. The workers process the messages in parallel, and by default (using ByteSum distribution strategy), messages with the same partition key are processed by the same worker, so that the message order is respected for the same partition key. Every worker has a buffer to avoid idling when many messages arrive with the same partition key for any other worker. The buffer size should be dimensioned depending on how many messages arrive with the same partition key, on average. When the bus is requested to stop, every worker receives the stop command and it only releases the stop call when it ends the current message and stores it in the [Offset Manager](#offset-manager).

### Middlewares

It’s a customizable collection of middlewares. This collection is configurable per consumer. Middlewares can be created by implementing the `IMessageMiddleware` interface. Each consumer has its own instances of middlewares, so, they aren't shared between consumers but they are shared between [Workers](#workers). You can see more information about middlewares [here](middlewares)

### Offset Manager

It is a component that receives all the offsets from the workers and orchestrates them before storing into Kafka; this avoids an offset override when we have many messages being processed at the same time. Even when you choose to use the manual offset store option, you will store the offset in the OffsetManager, and will then store the offsets in Kafka when possible. When the application stops, there is a big chance to have processed messages already stored in OffsetManager but not stored in Kafka. In this scenario, when the application starts again, these messages will be processed again. Your application must be prepared to deal with it.

## How it works

A sample consumer listening one topic with two [Workers](#workers) having a buffer size of 2 using the **BytesSum** distribution strategy.

![consumer-animation](https://user-images.githubusercontent.com/233064/98690723-22f3bc80-2365-11eb-8453-04349abb103c.gif)

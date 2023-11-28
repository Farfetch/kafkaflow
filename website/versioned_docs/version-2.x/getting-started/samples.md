---
sidebar_position: 5
---

# Samples

We know that working code is a valuable learning tool for many, so here you can find a list of samples built to demonstrate KafkaFlow capabilities.

## Basic

This is a simple sample that shows how to produce and consume messages.

You can find the code here: [/samples/KafkaFlow.Sample](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample)

## Batching

This is a sample that shows batch processing using KafkaFlow.

You can find the code here: [/samples/KafkaFlow.Sample.BatchOperations](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.BatchOperations)

## Schema Registry

This is a sample that shows how to use Schema Registry with KafkaFlow.

You can find the code here: [/samples/KafkaFlow.Sample](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.SchemaRegistry)

## Web API

This sample shows how to host an administration Web API for administrative operations.

You can find the code here: [/samples/KafkaFlow.Sample](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.WebApi)

## Dashboard

This sample shows how to use KafkaFlow to expose an administration Dashboard.

You can find the code here: [/samples/KafkaFlow.Sample](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.Dashboard)

## Flow of Control

This is a sample that shows how to control the state of a consumer (Starting, Pausing, Stopping, etc.) programmatically.

You can find the code here: [/samples/KafkaFlow.Sample.FlowControl](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.FlowControl)

## Pause Consumer on Error

This is a sample that shows how to stop the consumer when an exception is raised.

You can find the code here: [/samples/KafkaFlow.Sample.PauseConsumerOnError](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.PauseConsumerOnError)

## Consumer Throttling

This is a sample that shows how to throttle a consumer based on others consumers lag

You can find the code here: [/samples/KafkaFlow.Sample.ConsumerThrottling](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.ConsumerThrottling)

## Wildcard Consumers

This sample shows how to use a consumer to handle all the topics according to a naming convention. This is not a feature of KafkaFlow, but a demonstration of how to use the pattern conventions exposed by [librdkafka](https://github.com/confluentinc/librdkafka/tree/95a542c87c61d2c45b445f91c73dd5442eb04f3c) ([here](https://github.com/confluentinc/librdkafka/blob/95a542c87c61d2c45b445f91c73dd5442eb04f3c/src-cpp/rdkafkacpp.h#L2681)).

You can find the code here: [/samples/KafkaFlow.Sample.WildcardConsumer](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.WildcardConsumer)
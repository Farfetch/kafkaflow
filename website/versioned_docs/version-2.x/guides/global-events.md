---
sidebar_position: 9
---

# Global Events

In this section, we will delve into the concept of Global Events in KafkaFlow, which provides a mechanism to subscribe to various events that are triggered during the message production and consumption processes. 

KafkaFlow offers a range of Global Events that can be subscribed to. These events can be used to monitor and react to different stages of message handling. Below is a list of available events:

- [Message Produce Started Event](#message-produce-started-event)
- [Message Produce Completed Event](#message-produce-completed-event)
- [Message Produce Error Event](#message-produce-error-event)
- [Message Consume Started Event](#message-consume-started-event)
- [Message Consume Completed Event](#message-consume-completed-event)
- [Message Consume Error Event](#message-consume-error-event)

## Message Produce Started Event {#message-produce-started-event}

The Message Produce Started Event is triggered when the message production process begins. It provides an opportunity to perform tasks or gather information before middlewares execution.

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageProduceStarted.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```

## Message Produce Completed Event {#message-produce-completed-event}

The Message Produce Completed Event is triggered when a message is successfully produced or when error messages occur during the production process. Subscribing to this event enables you to track the successful completion of message production.

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageProduceCompleted.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```

## Message Produce Error Event {#message-produce-error-event}

In case an error occurs during message production, the Message Produce Error Event is triggered. By subscribing to this event, you will be able to catch any exceptions that may occur while producing a message.

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageProduceError.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```

## Message Consume Started Event {#message-consume-started-event}

The Message Consume Started Event is raised at the beginning of the message consumption process. It offers an opportunity to execute specific tasks or set up resources before message processing begins.

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageConsumeStarted.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```

## Message Consume Completed Event {#message-consume-completed-event}

The Message Consume Completed Event signals the successful completion of message consumption. By subscribing to this event, you can track when messages have been successfully processed.

:::info
Please note that the current event is not compatible with Batch Consume in the current version (v2). However, this limitation is expected to be addressed in future releases (v3+).
:::

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageProduceCompleted.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```

## Message Consume Error Event {#message-consume-error-event}

If an error occurs during message consumption, the Message Consume Error Event is triggered. Subscribing to this event allows you to manage and respond to consumption errors.

```csharp
services.AddKafka(
    kafka => kafka
        .SubscribeGlobalEvents(observers =>
        {
            observers.MessageConsumeError.Subscribe(eventContext =>
            {
                // Add your logic here
            });
        })
```
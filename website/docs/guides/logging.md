---
sidebar_position: 1
---
# Logging

In this section, we will learn how to configure Logging in KafkaFlow.


There are a few options you can use to add logging to KafkaFlow:
  - [Using Console Logger](#console-logger)
  - [Using Microsoft Logging Framework](#microsoft-logging-framework)
  - [Using your own Logger](#custom-logger)

:::info

By default, KafkaFlow logs are ignored.
The default implementation is the `NullLogHandler`. 

:::

## Using Console Logger {#console-logger}

The package [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/) can be installed to log the framework messages to the console output after the installation, use the method `UseConsoleLog` in the [configuration](../getting-started/configuration).

```csharp
services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        ...
```

## Using Microsoft Logging Framework {#microsoft-logging-framework}

The package [KafkaFlow.LogHandler.Microsoft](https://www.nuget.org/packages/KafkaFlow.LogHandler.Microsoft/) can be installed to log the framework messages to the console output after the installation, use the method `UseMicrosoftLog` in the [configuration](../getting-started/configuration).

```csharp
services.AddKafka(
    kafka => kafka
        .UseMicrosoftLog()
        ...
```

## Using your own Logger {#custom-logger}

The framework has the `ILogHandler` interface that can be implemented to log the framework's messages. The log handler can be configured in the [configuration](../getting-started/configuration) process using the method `UseLogHandler`:

```csharp
services.AddKafka(
    kafka => kafka
        .UseLogHandler<YourLogHandler>()
        ...
```

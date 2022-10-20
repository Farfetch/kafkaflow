---
sidebar_position: 7
---

# Log Handler

The framework has the `ILogHandler` interface that can be implemented to log the framework's messages. The log handler can be configured in the setup process using the method `UseLogHandler`:

```csharp
services.AddKafka(
    kafka => kafka
        .UseLogHandler<YourLogHandler>()
        ...
```

 **The default implementation is the `NullLogHandler` that ignores every log done by the framework**. The package [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/) can be installed to log the framework messages to the console output, after the installation, use the method `UseConsoleLog` in the setup.
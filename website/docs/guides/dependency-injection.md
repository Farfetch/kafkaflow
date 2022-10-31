---
sidebar_position: 6
---

# Dependency Injection

KafkaFlow can support any DI framework. We natively support [Microsoft DI](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/) and [Unity 5](https://www.nuget.org/packages/KafkaFlow.Unity/).

To support other DI frameworks you should implement the interfaces `IDependencyConfigurator`, `IDependencyResolver`, and `IDependencyResolverScope`.

### Unity or other DI framework

```csharp
var container = new UnityContainer();

var configurator = new KafkaFlowConfigurator(
    // Install KafkaFlow.Unity package
    new UnityDependencyConfigurator(container),
    kafka => kafka
        .AddCluster( ... )
);

//Call bus.StartAsync() when your app starts and bus.StopAsync() when your app stops
var bus = configurator.CreateBus(new UnityDependencyResolver(container));
```

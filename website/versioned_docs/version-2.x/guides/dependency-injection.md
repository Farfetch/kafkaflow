---
sidebar_position: 20
---

# Dependency Injection

KafkaFlow Dependency Injection framework support is extensible.

[Microsoft .NET DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/) and [Unity 5](http://unitycontainer.org/articles/quickstart.html) are natively supported. You can see [here](configuration) how to use them.

## Add support for a new Dependency Injection container

Other DI frameworks can be supported by implementing a set of interfaces:

- `IDependencyConfigurator`
- `IDependencyResolver`
- `IDependencyResolverScope`


You can find an example [here](https://github.com/Farfetch/kafkaflow/tree/master/src/KafkaFlow.Unity).

Once the interfaces are implemented, use them the same way you use Unity ([see here](configuration)).
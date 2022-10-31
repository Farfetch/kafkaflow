---
sidebar_position: 2
---

# Authentication

To produce and consume messages to/from authenticated brokers you have to configure the cluster with security information in the application setup.

KafkaFlow sends all the security information to [Confluent Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet) so more information about it can be found [here](https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md).

```csharp
  services.AddKafka(
    kafka => kafka
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] {"localhost:9092"})
                .WithSchemaRegistry(config => config.Url = "localhost:8081")
                .WithSecurityInformation(information =>
                {
                  information.SaslMechanism = SaslMechanism.Plain;
                  information.SaslPassword = "pwd";
                  information.SaslUsername = "user";
                  information.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                  information.EnableSslCertificateVerification = true;
                  ...
                })
                ...
            ...
```

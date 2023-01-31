---
sidebar_position: 1
description: KafkaFlow is a .NET framework to create Kafka-based applications, simple to use and extend.
sidebar_label: Introduction
slug: /
---

# Introduction to KafkaFlow

⚡️ KafkaFlow was designed to build .NET applications on top of Apache Kafka **in a simple and maintainable way**.

🏗 Built on top of [Confluent Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet).

🔌 Extensible by design.


Get started building by installing [KafkaFlow](getting-started/installation.md) or following our [Quickstart](getting-started/create-your-first-application.md).

## Features {#features}

Our goal is to empower you to build event-driven applications on top of Apache Kafka. 
To do that, KafkaFlow gives you access to features like:

- Multi-threaded consumer with message order guarantee.
- [Middlewares](guides/middlewares/) support for producing and consuming messages.
- Support topics with different message types.
- Consumers with many topics.
- [Serializer middleware](guides/middlewares/serializer-middleware.md) with **ApacheAvro**, **ProtoBuf** and **Json** algorithms.
- [Schema Registry](guides/middlewares/serializer-middleware.md#adding-schema-registry-support) support.
- [Compression](guides/compression.md) using native Confluent Kafka client compression or compressor middleware.
- Graceful shutdown (wait to finish processing to shutdown).
- Store offset when processing ends, avoiding message loss.
- Supports .NET Core and .NET Framework.
- Can be used with any dependency injection framework (see [here](guides/dependency-injection.md)).
- Fluent configuration.
- [Admin Web API](guides/admin/web-api.md) that allows pause, resume and restart consumers, change workers count, and rewind offsets, **all at runtime**.
- [Dashboard UI](guides/admin/dashboard.md) that allows to visualize of relevant information about all consumers and managing them.


## Join the community {#join-the-community}

- [GitHub](https://github.com/farfetch/kafkaflow): For new feature requests, bug reporting or contributing with your own pull request.
- [Slack](https://join.slack.com/t/kafkaflow/shared_invite/zt-puihrtcl-NnnylPZloAiVlQfsw~RD6Q): The place to be if you want to ask questions or share ideas.

## Something missing here? {#something-missing}

If you have suggestions to improve the documentation, please send us a new [issue](https://github.com/farfetch/kafkaflow/issues).


## License

KafkaFlow is a free and open source project, released under the permissible [MIT license](https://github.com/Farfetch/kafkaflow/blob/master/LICENSE). 
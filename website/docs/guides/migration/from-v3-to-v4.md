---
sidebar_position: 2
---

# From v3 to v4

KafkaFlow version 4 introduces the latest Confluent.Kafka package and upgrades to .NET 8. This guide will help you navigate the migration process from version 3 to version 4.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Update package references](#update-package-references)
- [Breaking Changes / Improvements](#breaking-changes--improvements)
  - [1. Update to .NET 8 with Admin Packages](#1-update-to-net-8-with-admin-packages)
  - [2. Update `Confluent.*` from `2.1.1` to `2.8.0`](#2-update-confluent-from-211-to-280)
- [Conclusion](#conclusion)

## Prerequisites

As .NET Core 6 has reached the end of support in its lifecycle, we have updated references to Core Packages (`Microsoft.*` and `System.*`) to version 8.

While the KafkaFlow core and most of the extension packages are still targeting `netstandard2.0` which supports a range of runtimes, we recommend with this v4 update to target at least .NET 8 in applications using KafkaFlow.

## Update package references

To update to KafkaFlow v4, change the `Version` related to KafkaFlow packages to the latest v4 available in each project referencing KafkaFlow packages.

```
<ItemGroup>
-   <PackageReference Include="KafkaFlow" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Abstractions" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Admin" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Admin.Dashboard" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Admin.WebApi" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Compressor.Gzip" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Extensions.Hosting" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.LogHandler.Console" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.LogHandler.Microsoft" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Microsoft.DependencyInjection" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.OpenTelemetry" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.SchemaRegistry" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.JsonCore" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.NewtonsoftJson" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.ProtobufNet" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentJson" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentProtobuf" Version="3.1.0" />
-   <PackageReference Include="KafkaFlow.Unity" Version="3.1.0" />

+   <PackageReference Include="KafkaFlow" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Abstractions" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Admin" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Admin.Dashboard" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Admin.WebApi" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Compressor.Gzip" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Extensions.Hosting" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.LogHandler.Console" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.LogHandler.Microsoft" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Microsoft.DependencyInjection" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.OpenTelemetry" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.SchemaRegistry" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.JsonCore" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.NewtonsoftJson" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.ProtobufNet" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentJson" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Serializer.SchemaRegistry.ConfluentProtobuf" Version="4.0.0" />
+   <PackageReference Include="KafkaFlow.Unity" Version="4.0.0" />
</ItemGroup>
```

## Breaking Changes / Improvements

The update to v4 introduces some breaking changes. Please consider them when updating KafkaFlow.

### 1. Update to .NET 8 with Admin Packages

> Related Issues: [#612](https://github.com/Farfetch/kafkaflow/issues/612)

The target frameworks for ASP.NET Admin projects have been updated from `.net6.0` to `.net8.0`.

If you are using the Admin packages (`KafkaFlow.Admin.Dashboard` and `KafkaFlow.Admin.WebApi`), you must target the .NET 8 runtime.

### 2. Update `Confluent.*` from `2.1.1` to `2.8.0`

All `Confluent.*` packages have been updated. No breaking changes are expected, but it is recommended to check the release notes for potential impacts: [Confluent Kafka Release Notes](https://github.com/confluentinc/confluent-kafka-dotnet/releases).

## Conclusion

Please ensure you review and adapt your codebase according to these changes. If you encounter any issues or need assistance, feel free to [reach out](https://github.com/Farfetch/kafkaflow#get-in-touch) to the KafkaFlow community.

Thank you for using KafkaFlow!

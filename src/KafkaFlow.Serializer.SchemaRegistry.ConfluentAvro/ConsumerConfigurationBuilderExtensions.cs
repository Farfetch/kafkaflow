﻿using Confluent.SchemaRegistry;
using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class ConsumerConfigurationBuilderExtensions
{
    /// <summary>
    /// Registers a middleware to deserialize avro messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <returns></returns>
    public static IConsumerMiddlewareConfigurationBuilder AddSchemaRegistryAvroDeserializer(
        this IConsumerMiddlewareConfigurationBuilder middlewares)
    {
        return middlewares.Add(
            resolver => new DeserializerConsumerMiddleware(
                new ConfluentAvroDeserializer(resolver),
                new SchemaRegistryTypeResolver(new ConfluentAvroTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
    }
}

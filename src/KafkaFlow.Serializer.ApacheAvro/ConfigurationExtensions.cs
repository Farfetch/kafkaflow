namespace KafkaFlow.Serializer.ApacheAvro
{
    using System;
    using Configuration;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    
    public static class ClusterConfigurationBuilderExtensions
    {
        public static IClusterConfigurationBuilder WithSchemaRegistry(
            this IClusterConfigurationBuilder cluster,
            Action<SchemaRegistryConfig> handler)
        {
            var config = new SchemaRegistryConfig();
            handler(config);
            cluster.DependencyConfigurator.AddTransient<ISchemaRegistryClient>(factory => new CachedSchemaRegistryClient(config));
            return cluster;
        }
        
        public static IProducerMiddlewareConfigurationBuilder AddApacheAvroSerializer(
            this IProducerMiddlewareConfigurationBuilder middlewares)
        {
            return middlewares.AddApacheAvroSerializer(new AvroSerializerConfig());
        }
        
        public static IProducerMiddlewareConfigurationBuilder AddApacheAvroSerializer<TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares) where TResolver : class, IMessageTypeResolver
        {
            return middlewares.AddApacheAvroSerializer<TResolver>(new AvroSerializerConfig());
        }
        
        public static IProducerMiddlewareConfigurationBuilder AddApacheAvroSerializer(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            AvroSerializerConfig serializerConfig)
        {
            middlewares.DependencyConfigurator.AddTransient<ApacheAvroMessageSerializer>();
            
            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    new ApacheAvroMessageSerializer( 
                        resolver.Resolve<ISchemaRegistryClient>(),
                        serializerConfig),
                    new DefaultMessageTypeResolver()));
        }
        
        public static IProducerMiddlewareConfigurationBuilder AddApacheAvroSerializer<TResolver>(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            AvroSerializerConfig serializerConfig) where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddTransient<TResolver>();
            middlewares.DependencyConfigurator.AddTransient<ApacheAvroMessageSerializer>();
            
            return middlewares.Add(
                resolver => new SerializerProducerMiddleware(
                    new ApacheAvroMessageSerializer( 
                        resolver.Resolve<ISchemaRegistryClient>(),
                        serializerConfig),
                    resolver.Resolve<TResolver>()));
        }

        public static IConsumerMiddlewareConfigurationBuilder AddApacheAvroSerializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            middlewares.DependencyConfigurator.AddTransient<ApacheAvroMessageSerializer>();
            
            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    new ApacheAvroMessageSerializer(resolver.Resolve<ISchemaRegistryClient>()),
                    new DefaultMessageTypeResolver()));
        }
        
        public static IConsumerMiddlewareConfigurationBuilder AddApacheAvroSerializer<TResolver>(
            this IConsumerMiddlewareConfigurationBuilder middlewares) where TResolver : class, IMessageTypeResolver
        {
            middlewares.DependencyConfigurator.AddTransient<ApacheAvroMessageSerializer>();
            
            return middlewares.Add(
                resolver => new SerializerConsumerMiddleware(
                    new ApacheAvroMessageSerializer(resolver.Resolve<ISchemaRegistryClient>()),
                    resolver.Resolve<TResolver>()));
        } 
    }
}

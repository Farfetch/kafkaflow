namespace KafkaFlow.TypedHandler
{
    using System;
    using KafkaFlow.Configuration;

    public static class ConfigurationBuilderExtensions
    {
        public static IConsumerMiddlewareConfigurationBuilder AddTypedHandlers(
            this IConsumerMiddlewareConfigurationBuilder middlewares,
            Action<TypedHandlerConfigurationBuilder> configure)
        {
            var builder = new TypedHandlerConfigurationBuilder(middlewares.DependencyConfigurator);

            configure(builder);

            var configuration = builder.Build();

            middlewares.DependencyConfigurator.AddSingleton(configuration);
            middlewares.Add(resolver => new TypedHandlerMiddleware(resolver, configuration));

            return middlewares;
        }
    }
}

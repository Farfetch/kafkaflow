namespace KafkaFlow.TypedHandler
{
    using System;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods over <see cref="IConsumerMiddlewareConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds typed handler middlewares into the stack of middlewares 
        /// </summary>
        /// <param name="middlewaresBuilder">Instance of <see cref="IConsumerMiddlewareConfigurationBuilder"/></param>
        /// <param name="configure">Action to be done over instance of TypedHandlerConfigurationBuilder</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddTypedHandlers(
            this IConsumerMiddlewareConfigurationBuilder middlewaresBuilder,
            Action<TypedHandlerConfigurationBuilder> configure)
        {
            var typedHandlerBuilder = new TypedHandlerConfigurationBuilder(middlewaresBuilder.DependencyConfigurator);

            configure(typedHandlerBuilder);

            var configuration = typedHandlerBuilder.Build();

            middlewaresBuilder.DependencyConfigurator.AddSingleton(configuration);
            middlewaresBuilder.Add(resolver => new TypedHandlerMiddleware(resolver, configuration));

            return middlewaresBuilder;
        }
    }
}

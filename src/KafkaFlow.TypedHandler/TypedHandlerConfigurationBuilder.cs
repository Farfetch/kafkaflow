namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TypedHandlerConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<Type> handlers = new List<Type>();

        private InstanceLifetime serviceLifetime = InstanceLifetime.Singleton;

        public TypedHandlerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        /// <summary>
        /// Adds all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlersFromAssemblyOf<T>()
            where T : IMessageHandler
        {
            var handlersType = typeof(T).Assembly
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(x));

            this.handlers.AddRange(handlersType);

            return this;
        }

        /// <summary>
        /// Manually adds the message handlers
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlers(IEnumerable<Type> handlers)
        {
            this.handlers.AddRange(handlers);
            return this;
        }

        /// <summary>
        /// Manually adds the message handler
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandler<T>()
            where T : class, IMessageHandler
        {
            this.handlers.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Set the handler lifetime. The default value is <see cref="InstanceLifetime.Singleton"/>
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder WithHandlerLifetime(InstanceLifetime lifetime)
        {
            this.serviceLifetime = lifetime;
            return this;
        }

        public TypedHandlerConfiguration Build()
        {
            var configuration = new TypedHandlerConfiguration();

            foreach (var handlerType in this.handlers)
            {
                this.dependencyConfigurator.Add(
                    handlerType,
                    handlerType,
                    this.serviceLifetime);

                var messageTypes = handlerType
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && typeof(IMessageHandler).IsAssignableFrom(x))
                    .Select(x => x.GenericTypeArguments[0]);

                foreach (var messageType in messageTypes)
                {
                    configuration.HandlerMapping.AddMapping(messageType, handlerType);
                }
            }

            return configuration;
        }
    }
}

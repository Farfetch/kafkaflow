using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using KafkaFlow.Middlewares.TypedHandler.Configuration;

namespace KafkaFlow
{
    /// <summary>
    /// Builder class for typed handler configuration
    /// </summary>
    public class TypedHandlerConfigurationBuilder
    {
        private readonly IDependencyConfigurator _dependencyConfigurator;
        private readonly List<Type> _handlers = new();

        private Action<IMessageContext> _onNoHandlerFound = (_) => { };
        private InstanceLifetime _serviceLifetime = InstanceLifetime.Singleton;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedHandlerConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        public TypedHandlerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            _dependencyConfigurator = dependencyConfigurator;
        }

        /// <summary>
        /// Adds all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlersFromAssemblyOf<T>() =>
            this.AddHandlersFromAssemblyOf(typeof(T));

        /// <summary>
        /// Adds all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assemblies of the provided types
        /// </summary>
        /// <param name="assemblyMarkerTypes">Types contained in the assemblies to be searched</param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlersFromAssemblyOf(params Type[] assemblyMarkerTypes)
        {
            var handlerTypes = assemblyMarkerTypes
                .SelectMany(t => t.GetTypeInfo().Assembly.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(x))
                .Distinct();

            _handlers.AddRange(handlerTypes);
            return this;
        }

        /// <summary>
        /// Manually adds the message handlers
        /// </summary>
        /// <param name="handlers">The handlers to execute the messages</param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlers(IEnumerable<Type> handlers)
        {
            _handlers.AddRange(handlers);
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
            _handlers.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Register the action to be executed when no handler was found to process the message
        /// </summary>
        /// <param name="handler">The handler that will be executed</param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder WhenNoHandlerFound(Action<IMessageContext> handler)
        {
            _onNoHandlerFound = handler;
            return this;
        }

        /// <summary>
        /// Set the handler lifetime. The default value is <see cref="InstanceLifetime.Singleton"/>
        /// </summary>
        /// <param name="lifetime">The <see cref="InstanceLifetime"/> enum value</param>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder WithHandlerLifetime(InstanceLifetime lifetime)
        {
            _serviceLifetime = lifetime;
            return this;
        }

        internal TypedHandlerConfiguration Build()
        {
            var configuration = new TypedHandlerConfiguration
            {
                OnNoHandlerFound = _onNoHandlerFound,
            };

            foreach (var handlerType in _handlers)
            {
                _dependencyConfigurator.Add(
                    handlerType,
                    handlerType,
                    _serviceLifetime);

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

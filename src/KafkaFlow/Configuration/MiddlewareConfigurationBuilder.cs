namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    internal class MiddlewareConfigurationBuilder<TBuilder>
        : IMiddlewareConfigurationBuilder<TBuilder>
        where TBuilder : class, IMiddlewareConfigurationBuilder<TBuilder>
    {
        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories = new();

        public MiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public TBuilder Add<T>(Factory<T> factory)
            where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Add(factory);
            return this as TBuilder;
        }

        public TBuilder AddAtBeginning<T>(Factory<T> factory)
            where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Insert(0, factory);
            return this as TBuilder;
        }

        public TBuilder Add<T>()
            where T : class, IMessageMiddleware
        {
            this.RegisterType<T>();
            this.middlewaresFactories.Add(resolver => resolver.Resolve<T>());
            return this as TBuilder;
        }

        public TBuilder AddAtBeginning<T>()
            where T : class, IMessageMiddleware
        {
            this.RegisterType<T>();
            this.middlewaresFactories.Insert(0, resolver => resolver.Resolve<T>());
            return this as TBuilder;
        }

        public MiddlewareConfiguration Build() => new(this.middlewaresFactories);

        private void RegisterType<T>()
            where T : class, IMessageMiddleware
        {
            this.DependencyConfigurator.AddTransient<T>();
        }
    }
}

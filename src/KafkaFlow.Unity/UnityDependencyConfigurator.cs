namespace KafkaFlow.Unity
{
    using System;
    using global::Unity;
    using global::Unity.Lifetime;
    using InstanceLifetime = KafkaFlow.InstanceLifetime;

    internal class UnityDependencyConfigurator : IDependencyConfigurator
    {
        private readonly IUnityContainer container;

        public UnityDependencyConfigurator(IUnityContainer container)
        {
            this.container = container;
        }

        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            InstanceLifetime lifetime)
        {
            this.container.RegisterType(
                serviceType,
                implementationType,
                (ITypeLifetimeManager) ParseLifetime(lifetime));
            return this;
        }

        public IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class where TImplementation : class, TService
        {
            this.container.RegisterType<TService, TImplementation>((ITypeLifetimeManager) ParseLifetime(lifetime));
            return this;
        }

        public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime) where TService : class
        {
            this.container.RegisterType<TService>((ITypeLifetimeManager) ParseLifetime(lifetime));
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            this.container.RegisterInstance(service);
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime)
        {
            this.container.RegisterFactory(
                serviceType,
                c => factory(new UnityDependencyResolver(c)),
                (IFactoryLifetimeManager) ParseLifetime(lifetime));

            return this;
        }

        private static object ParseLifetime(InstanceLifetime lifetime) =>
            lifetime switch
            {
                InstanceLifetime.Singleton => new ContainerControlledLifetimeManager(),
                InstanceLifetime.Scoped => new HierarchicalLifetimeManager(),
                InstanceLifetime.Transient => new TransientLifetimeManager(),
                _ => throw new InvalidCastException($"There is not mapping defined to {lifetime}")
            };
    }
}

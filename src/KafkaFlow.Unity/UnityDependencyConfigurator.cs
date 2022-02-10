namespace KafkaFlow.Unity
{
    using System;
    using System.Linq;
    using global::Unity;
    using global::Unity.Lifetime;
    using InstanceLifetime = KafkaFlow.InstanceLifetime;

    /// <summary>
    /// The Unity implementation of <see cref="IDependencyConfigurator"/>
    /// </summary>
    public class UnityDependencyConfigurator : IDependencyConfigurator
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyConfigurator"/> class.
        /// </summary>
        /// <param name="container">The Unity container instance</param>
        public UnityDependencyConfigurator(IUnityContainer container)
        {
            this.container = container;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            InstanceLifetime lifetime)
        {
            this.container.RegisterType(
                serviceType,
                implementationType,
                (ITypeLifetimeManager)ParseLifetime(lifetime));
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            this.container.RegisterType<TService, TImplementation>((ITypeLifetimeManager)ParseLifetime(lifetime));
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class
        {
            this.container.RegisterType<TService>((ITypeLifetimeManager)ParseLifetime(lifetime));
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            this.container.RegisterInstance(service);
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime)
        {
            string name = null;

            if (this.AlreadyRegistered(serviceType))
            {
                name = Guid.NewGuid().ToString();
            }

            this.container.RegisterFactory(
                serviceType,
                name,
                c => factory(new UnityDependencyResolver(c)),
                (IFactoryLifetimeManager)ParseLifetime(lifetime));

            return this;
        }

        private bool AlreadyRegistered(Type registeredType)
        {
            return this.container.Registrations.Any(x => x.RegisteredType == registeredType);
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

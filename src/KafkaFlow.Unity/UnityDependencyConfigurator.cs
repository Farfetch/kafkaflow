using System;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace KafkaFlow.Unity
{
    /// <summary>
    /// The Unity implementation of <see cref="IDependencyConfigurator"/>
    /// </summary>
    public class UnityDependencyConfigurator : IDependencyConfigurator
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyConfigurator"/> class.
        /// </summary>
        /// <param name="container">The Unity container instance</param>
        public UnityDependencyConfigurator(IUnityContainer container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            InstanceLifetime lifetime)
        {
            _container.RegisterType(
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
            _container.RegisterType<TService, TImplementation>((ITypeLifetimeManager)ParseLifetime(lifetime));
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class
        {
            _container.RegisterType<TService>((ITypeLifetimeManager)ParseLifetime(lifetime));
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            _container.RegisterInstance(service);
            return this;
        }

        /// <inheritdoc />
        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime)
        {
            string name = null;

            if (AlreadyRegistered(serviceType))
            {
                name = Guid.NewGuid().ToString();
            }

            _container.RegisterFactory(
                serviceType,
                name,
                c => factory(new UnityDependencyResolver(c)),
                (IFactoryLifetimeManager)ParseLifetime(lifetime));

            return this;
        }

        /// <inheritdoc />
        public bool AlreadyRegistered(Type registeredType)
        {
            return _container.Registrations.Any(x => x.RegisteredType == registeredType);
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

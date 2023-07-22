namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Represents the interface to be implemented by custom dependency configurator
    /// </summary>
    public interface IDependencyConfigurator
    {
        /// <summary>
        /// Registers a type mapping where the created instances will use the given <see cref="InstanceLifetime"/>
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned</param>
        /// <param name="lifetime">The <see cref="InstanceLifetime"/> that controls the lifetime of the returned instance</param>
        /// <returns>The <see cref="IDependencyConfigurator"/> object that this method was called on</returns>
        IDependencyConfigurator Add(Type serviceType, Type implementationType, InstanceLifetime lifetime);

        /// <summary>
        /// Registers a type mapping where the created instances will use the given <see cref="InstanceLifetime"/>.
        /// </summary>
        /// <param name="lifetime">The <see cref="InstanceLifetime"/> that controls the lifetime of the returned instance</param>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned</typeparam>
        /// <returns>The <see cref="IDependencyConfigurator"/> object that this method was called on</returns>
        IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Registers a type where the created instance will use the given <see cref="InstanceLifetime"/>.
        /// </summary>
        /// <param name="lifetime">The <see cref="InstanceLifetime"/> that controls the lifetime of the returned instance</param>
        /// <typeparam name="TService"><see cref="Type"/> that will be created</typeparam>
        /// <returns>The <see cref="IDependencyConfigurator"/> object that this method was called on</returns>
        IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class;

        /// <summary>
        /// Registers a singleton type mapping where the returned instance will be the given <typeparam name="TImplementation"/> implementation.
        /// </summary>
        /// <returns>The <see cref="IDependencyConfigurator"/> object that this method was called on</returns>
        IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class;

        /// <summary>
        /// Registers a type mapping where the returned instance will be the given by the provided factory.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested</param>
        /// <param name="factory">A factory to create new instances of the service implementation</param>
        /// <param name="lifetime">The <see cref="InstanceLifetime"/> that controls the lifetime of the returned instance</param>
        /// <returns>The <see cref="IDependencyConfigurator"/> object that this method was called on</returns>
        IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime);

        /// <summary>
        /// Checks if the given <paramref name="registeredType" /> is already registered
        /// </summary>
        /// <param name="registeredType">Service type</param>
        /// <returns></returns>
        bool AlreadyRegistered(Type registeredType);
    }
}

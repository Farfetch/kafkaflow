namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Provides extension methods over <see cref="IDependencyConfigurator"/>
    /// </summary>
    public static class DependencyConfiguratorExtensions
    {
        /// <summary>
        /// Registers a singleton type mapping
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on.</param>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddSingleton<TService, TImplementation>(this IDependencyConfigurator configurator)
            where TImplementation : class, TService
            where TService : class
        {
            return configurator.Add<TService, TImplementation>(InstanceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a singleton type mapping
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <typeparam name="TService"><see cref="Type"/> that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddSingleton<TService>(this IDependencyConfigurator configurator)
            where TService : class
        {
            return configurator.Add<TService>(InstanceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a singleton type mapping where the returned instance will be the given implementation
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <param name="service"><see cref="Type"/> that will be returned</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddSingleton<TService>(
            this IDependencyConfigurator configurator,
            TService service)
            where TService : class
        {
            return configurator.Add(service);
        }

        /// <summary>
        /// Registers a singleton type mapping where the returned instance will be given by the provided factory
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <param name="factory">A factory to create new instances of the service implementation</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddSingleton<TService>(
            this IDependencyConfigurator configurator,
            Func<IDependencyResolver, TService> factory)
        {
            return configurator.Add(
                typeof(TService),
                factory,
                InstanceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a scoped type mapping where the returned instance will be given by the provided factory
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddScoped<TService>(this IDependencyConfigurator configurator)
            where TService : class
        {
            return configurator.Add<TService>(InstanceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a scoped type mapping where the returned instance will be given by the provided factory
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <param name="factory">A factory to create new instances of the service implementation</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddScoped<TService>(
            this IDependencyConfigurator configurator,
            Func<IDependencyResolver, TService> factory)
        {
            return configurator.Add(
                typeof(TService),
                factory,
                InstanceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a transient type mapping
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <param name="serviceType"><see cref="Type"/> that will be requested</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned</param>
        /// <returns></returns>
        public static IDependencyConfigurator AddTransient(
            this IDependencyConfigurator configurator,
            Type serviceType,
            Type implementationType)
        {
            return configurator.Add(
                serviceType,
                implementationType,
                InstanceLifetime.Transient);
        }

        /// <summary>
        /// Registers a transient type mapping
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddTransient<TService>(this IDependencyConfigurator configurator)
            where TService : class
        {
            return configurator.Add<TService>(InstanceLifetime.Transient);
        }

        /// <summary>
        /// Registers a transient type mapping where the returned instance will be given by the provided factory
        /// </summary>
        /// <param name="configurator">The <see cref="IDependencyConfigurator"/> object that this method was called on</param>
        /// <param name="factory">A factory to create new instances of the service implementation</param>
        /// <typeparam name="TService">Type that will be created</typeparam>
        /// <returns></returns>
        public static IDependencyConfigurator AddTransient<TService>(
            this IDependencyConfigurator configurator,
            Func<IDependencyResolver, TService> factory)
        {
            return configurator.Add(
                typeof(TService),
                factory,
                InstanceLifetime.Transient);
        }
    }
}

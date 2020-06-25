namespace KafkaFlow
{
    using System;

    public static class DependencyConfiguratorExtensions
    {
        public static IDependencyConfigurator AddSingleton<TService, TImplementation>(
            this IDependencyConfigurator configurator)
            where TImplementation : class, TService
            where TService : class
        {
            return configurator.Add<TService, TImplementation>(InstanceLifetime.Singleton);
        }

        public static IDependencyConfigurator AddSingleton<TService>(this IDependencyConfigurator configurator)
            where TService : class
        {
            return configurator.Add<TService>(InstanceLifetime.Singleton);
        }

        public static IDependencyConfigurator AddSingleton<TService>(
            this IDependencyConfigurator configurator,
            TService service)
            where TService : class
        {
            return configurator.Add(service);
        }

        public static IDependencyConfigurator AddSingleton<TImplementation>(
            this IDependencyConfigurator configurator,
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory)
            where TImplementation : class
        {
            return configurator.Add(
                serviceType,
                factory,
                InstanceLifetime.Singleton);
        }

        public static IDependencyConfigurator AddSingleton(
            this IDependencyConfigurator configurator,
            Type serviceType,
            Type implementationType)
        {
            return configurator.Add(
                serviceType,
                implementationType,
                InstanceLifetime.Singleton);
        }

        public static IDependencyConfigurator AddTransient<TService>(this IDependencyConfigurator configurator)
            where TService : class
        {
            return configurator.Add<TService>(InstanceLifetime.Transient);
        }
    }
}

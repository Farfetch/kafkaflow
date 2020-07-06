namespace KafkaFlow
{
    /// <summary>
    /// Provides extension methods over <see cref="IDependencyResolver"/>
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Resolve an instance of <typeparamref name="T" />.
        /// </summary>
        /// <param name="resolver">Instance of <see cref="IDependencyResolver"/></param>
        /// <returns></returns>
        public static T Resolve<T>(this IDependencyResolver resolver) => (T) resolver.Resolve(typeof(T));
    }
}

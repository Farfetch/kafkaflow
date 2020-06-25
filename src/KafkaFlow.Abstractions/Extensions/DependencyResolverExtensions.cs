namespace KafkaFlow
{
    public static class DependencyResolverExtensions
    {
        public static T Resolve<T>(this IDependencyResolver resolver) => (T) resolver.Resolve(typeof(T));
    }
}

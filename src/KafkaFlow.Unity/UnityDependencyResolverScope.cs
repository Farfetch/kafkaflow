using global::Unity;

namespace KafkaFlow.Unity
{
    internal class UnityDependencyResolverScope : IDependencyResolverScope
    {
        private readonly IUnityContainer _container;

        public UnityDependencyResolverScope(IUnityContainer container)
        {
            _container = container;
            this.Resolver = new UnityDependencyResolver(container);
        }

        public IDependencyResolver Resolver { get; }

        public void Dispose() => _container.Dispose();
    }
}

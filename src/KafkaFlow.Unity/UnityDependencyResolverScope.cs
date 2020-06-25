namespace KafkaFlow.Unity
{
    using global::Unity;

    public class UnityDependencyResolverScope : IDependencyResolverScope
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolverScope(IUnityContainer container)
        {
            this.container = container;
            this.Resolver = new UnityDependencyResolver(container);
        }

        public void Dispose() => this.container.Dispose();

        public IDependencyResolver Resolver { get; }
    }
}

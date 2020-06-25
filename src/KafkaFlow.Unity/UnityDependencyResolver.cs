namespace KafkaFlow.Unity
{
    using System;
    using global::Unity;

    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolver(IUnityContainer container) => this.container = container;

        public IDependencyResolverScope CreateScope() =>
            new UnityDependencyResolverScope(this.container.CreateChildContainer());

        public object Resolve(Type type) => this.container.Resolve(type);
    }
}

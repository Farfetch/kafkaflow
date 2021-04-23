namespace KafkaFlow.Unity
{
    using System;
    using global::Unity;

    /// <summary>
    /// Unity implementation of <see cref="IDependencyResolver"/>
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">A Unity container instance</param>
        public UnityDependencyResolver(IUnityContainer container) => this.container = container;

        /// <summary>
        /// Creates a container scope
        /// </summary>
        /// <returns></returns>
        public IDependencyResolverScope CreateScope() =>
            new UnityDependencyResolverScope(this.container.CreateChildContainer());

        /// <summary>
        /// Gets an instances of the passed type
        /// </summary>
        /// <param name="type">The type to be created</param>
        /// <returns></returns>
        public object Resolve(Type type) => this.container.Resolve(type);
    }
}

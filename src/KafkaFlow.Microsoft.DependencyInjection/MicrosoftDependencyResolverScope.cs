namespace KafkaFlow.Microsoft.DependencyInjection
{
    using global::Microsoft.Extensions.DependencyInjection;

    internal class MicrosoftDependencyResolverScope : IDependencyResolverScope
    {
        private readonly IServiceScope scope;

        public MicrosoftDependencyResolverScope(IServiceScope scope)
        {
            this.scope = scope;
            this.Resolver = new MicrosoftDependencyResolver(scope.ServiceProvider);
        }

        public void Dispose()
        {
            this.scope.Dispose();
        }

        public IDependencyResolver Resolver { get; }
    }
}

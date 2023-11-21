using global::Microsoft.Extensions.DependencyInjection;

namespace KafkaFlow
{
    internal class MicrosoftDependencyResolverScope : IDependencyResolverScope
    {
        private readonly IServiceScope _scope;

        public MicrosoftDependencyResolverScope(IServiceScope scope)
        {
            _scope = scope;
            this.Resolver = new MicrosoftDependencyResolver(scope.ServiceProvider);
        }

        public IDependencyResolver Resolver { get; }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}

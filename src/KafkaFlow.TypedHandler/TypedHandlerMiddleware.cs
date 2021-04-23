namespace KafkaFlow.TypedHandler
{
    using System.Linq;
    using System.Threading.Tasks;

    internal class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly TypedHandlerConfiguration configuration;

        public TypedHandlerMiddleware(
            IDependencyResolver dependencyResolver,
            TypedHandlerConfiguration configuration)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            using (var scope = this.dependencyResolver.CreateScope())
            {
                await Task.WhenAll(
                        this.configuration
                            .HandlerMapping
                            .GetHandlersTypes(context.Message.GetType())
                            .Select(
                                handler =>
                                    HandlerExecutor
                                        .GetExecutor(context.Message.GetType())
                                        .Execute(
                                            scope.Resolver.Resolve(handler),
                                            context,
                                            context.Message)))
                    .ConfigureAwait(false);
            }

            await next(context).ConfigureAwait(false);
        }
    }
}

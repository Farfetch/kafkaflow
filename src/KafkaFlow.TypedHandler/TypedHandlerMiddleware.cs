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
            var handlers = this.configuration
                .HandlerMapping
                .GetHandlersTypes(context.Message.Value.GetType());

            if (!handlers.Any())
            {
                this.configuration.OnNoHandlerFound(context);
            }
            else
            {
                await Task.WhenAll(
                        handlers
                            .Select(
                                handler =>
                                    HandlerExecutor
                                        .GetExecutor(context.Message.Value.GetType())
                                        .Execute(
                                            this.dependencyResolver.Resolve(handler),
                                            context,
                                            context.Message.Value)))
                    .ConfigureAwait(false);
            }

            await next(context).ConfigureAwait(false);
        }
    }
}

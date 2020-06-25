namespace KafkaFlow.TypedHandler
{
    using System.Threading.Tasks;

    public class TypedHandlerMiddleware : IMessageMiddleware
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
                var handlerType = this.configuration.HandlerMapping.GetHandlerType(context.Message.GetType());

                if (handlerType == null)
                {
                    return;
                }

                var handler = scope.Resolver.Resolve(handlerType);

                await HandlerExecutor
                    .GetExecutor(context.Message.GetType())
                    .Execute(
                        handler,
                        context,
                        context.Message)
                    .ConfigureAwait(false);
            }

            await next(context);
        }
    }
}

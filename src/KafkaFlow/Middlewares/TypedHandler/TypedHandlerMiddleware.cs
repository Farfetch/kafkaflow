using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Middlewares.TypedHandler.Configuration;

namespace KafkaFlow.Middlewares.TypedHandler
{
    internal class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly TypedHandlerConfiguration _configuration;

        public TypedHandlerMiddleware(
            IDependencyResolver dependencyResolver,
            TypedHandlerConfiguration configuration)
        {
            _dependencyResolver = dependencyResolver;
            _configuration = configuration;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var handlers = _configuration
                .HandlerMapping
                .GetHandlersTypes(context.Message.Value?.GetType());

            if (!handlers.Any())
            {
                _configuration.OnNoHandlerFound(context);
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
                                            _dependencyResolver.Resolve(handler),
                                            context,
                                            context.Message.Value)))
                    .ConfigureAwait(false);
            }

            await next(context).ConfigureAwait(false);
        }
    }
}

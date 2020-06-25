namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly IReadOnlyList<IMessageMiddleware> middlewares;

        public MiddlewareExecutor(IReadOnlyList<IMessageMiddleware> middlewares)
        {
            this.middlewares = middlewares;
        }

        public Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation)
        {
            return this.ExecuteDefinition(
                0,
                context,
                nextOperation);
        }

        private Task ExecuteDefinition(
            int index,
            IMessageContext context,
            Func<IMessageContext, Task> nextOperation)
        {
            if (this.middlewares.Count == index)
            {
                return nextOperation(context);
            }

            return this.middlewares[index]
                .Invoke(
                    context,
                    nextContext => this.ExecuteDefinition(
                        index + 1,
                        nextContext.Clone(),
                        nextOperation));
        }
    }
}

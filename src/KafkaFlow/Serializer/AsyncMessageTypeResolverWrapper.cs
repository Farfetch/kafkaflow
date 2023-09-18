namespace KafkaFlow.Serializer
{
    using System;
    using System.Threading.Tasks;

    internal class AsyncMessageTypeResolverWrapper : IAsyncMessageTypeResolver
    {
        private readonly IMessageTypeResolver typeResolver;

        public AsyncMessageTypeResolverWrapper(IMessageTypeResolver typeResolver) => this.typeResolver = typeResolver;

        public Task<Type> OnConsumeAsync(IMessageContext context) =>
            Task.FromResult(this.typeResolver.OnConsume(context));

        public Task OnProduceAsync(IMessageContext context)
        {
            this.typeResolver.OnProduce(context);
            return Task.CompletedTask;
        }
    }
}

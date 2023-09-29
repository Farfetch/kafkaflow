namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly IReadOnlyList<MiddlewareConfiguration> configurations;

        private readonly Dictionary<int, IMessageMiddleware> consumerOrProducerMiddlewares = new();
        private readonly Dictionary<(int, int), IMessageMiddleware> workersMiddlewares = new();

        public MiddlewareExecutor(IReadOnlyList<MiddlewareConfiguration> configurations)
        {
            this.configurations = configurations;
        }

        public Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation)
        {
            return this.ExecuteDefinition(0, context, nextOperation);
        }

        public Task OnWorkerPoolStopped()
        {
            this.workersMiddlewares.Clear();
            return Task.CompletedTask;
        }

        private static IMessageMiddleware CreateInstance(
            IDependencyResolver dependencyResolver,
            MiddlewareConfiguration configuration)
        {
            if (configuration.InstanceContainerId is null)
            {
                return (IMessageMiddleware)dependencyResolver.Resolve(configuration.Type);
            }

            var instanceContainer = dependencyResolver
                .ResolveAll(typeof(MiddlewareInstanceContainer<>).MakeGenericType(configuration.Type))
                .Cast<IMiddlewareInstanceContainer>()
                .FirstOrDefault(x => x.Id == configuration.InstanceContainerId);

            if (instanceContainer is null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(MiddlewareConfiguration.InstanceContainerId),
                    configuration.InstanceContainerId,
                    "There is no instance container registered with the given ID. It's a bug! Please, open an issue!");
            }

            return instanceContainer.GetInstance(dependencyResolver);
        }

        private Task ExecuteDefinition(int index, IMessageContext context, Func<IMessageContext, Task> nextOperation)
        {
            if (this.configurations.Count == index)
            {
                return nextOperation(context);
            }

            var configuration = this.configurations[index];

            return this
                .ResolveInstance(index, context, configuration)
                .Invoke(
                    context,
                    nextContext => this.ExecuteDefinition(index + 1, nextContext, nextOperation));
        }

        private IMessageMiddleware ResolveInstance(int index, IMessageContext context, MiddlewareConfiguration configuration)
        {
            return configuration.Lifetime switch
            {
                MiddlewareLifetime.Worker => this.GetWorkerInstance(
                    index,
                    context,
                    configuration),
                MiddlewareLifetime.ConsumerOrProducer => this.GetConsumerOrProducerInstance(
                    context.ConsumerContext?.ConsumerDependencyResolver ?? context.ProducerContext?.ProducerDependencyResolver,
                    index,
                    configuration),
                _ => CreateInstance(context.DependencyResolver, configuration)
            };
        }

        private IMessageMiddleware GetConsumerOrProducerInstance(
            IDependencyResolver dependencyResolver,
            int index,
            MiddlewareConfiguration configuration)
        {
            return this.consumerOrProducerMiddlewares.SafeGetOrAdd(
                index,
                _ => CreateInstance(dependencyResolver, configuration));
        }

        private IMessageMiddleware GetWorkerInstance(
            int index,
            IMessageContext context,
            MiddlewareConfiguration configuration)
        {
            return this.workersMiddlewares.SafeGetOrAdd(
                (index, context.ConsumerContext?.WorkerId ?? 0),
                _ => CreateInstance(context.ConsumerContext.WorkerDependencyResolver, configuration));
        }
    }
}

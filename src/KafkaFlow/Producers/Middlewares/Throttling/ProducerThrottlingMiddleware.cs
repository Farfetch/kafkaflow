namespace KafkaFlow.Producers.Middlewares.Throttling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;
    using KafkaFlow.Producers.Middlewares.Throttling.Configuration;

    internal class ProducerThrottlingMiddleware : IMessageMiddleware
    {
        private readonly ProducerEvaluationConfiguration configuration;

        private IAction currentAction;

        public ProducerThrottlingMiddleware(
            IKafkaBus kafkaBus,
            IProducerConfiguration producerConfiguration,
            ProducerEvaluationConfiguration configuration)
        {
            this.configuration = configuration;

            Task.Run(() => this.Evaluate(producerConfiguration, kafkaBus.BusStopping));
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var localCurrentAction = this.currentAction;

            if (localCurrentAction is not null)
            {
                await localCurrentAction.HandleAsync(context).ConfigureAwait(false);
            }

            await next(context).ConfigureAwait(false);
        }

        private async Task Evaluate(
            IProducerConfiguration producerConfiguration,
            CancellationToken busStopping)
        {
            try
            {
                while (!busStopping.IsCancellationRequested)
                {
                    await Task
                        .Delay(this.configuration.Interval, busStopping)
                        .ConfigureAwait(false);

                    var action = await this.configuration.Evaluation
                        .EvaluateAsync(producerConfiguration, this.configuration.Actions, busStopping)
                        .ConfigureAwait(false);

                    if (this.currentAction is not null && this.currentAction != action)
                    {
                        await this.currentAction
                            .OnEndAsync(producerConfiguration, busStopping)
                            .ConfigureAwait(false);
                    }

                    Interlocked.Exchange(ref this.currentAction, action);
                }
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
        }
    }
}

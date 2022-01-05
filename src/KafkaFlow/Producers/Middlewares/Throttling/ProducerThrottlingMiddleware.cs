namespace KafkaFlow.Producers.Middlewares.Throttling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;
    using KafkaFlow.Producers.Middlewares.Throttling.Configuration;

    internal class ProducerThrottlingMiddleware : IMessageMiddleware
    {
        private readonly SemaphoreSlim evaluationSemaphore = new(1, 1);
        private readonly ProducerEvaluationConfiguration configuration;

        private IAction currentAction;
        private long lastEvaluation;

        public ProducerThrottlingMiddleware(ProducerEvaluationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (DateTime.Now.Ticks - this.lastEvaluation >= this.configuration.Interval.Ticks)
            {
                await this.evaluationSemaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (DateTime.Now.Ticks - this.lastEvaluation >= this.configuration.Interval.Ticks)
                    {
                        var action = await this.configuration.Evaluation
                            .EvaluateAsync(context, this.configuration.Actions)
                            .ConfigureAwait(false);

                        if (this.currentAction is not null && this.currentAction != action)
                        {
                            await this.currentAction.OnEndAsync(context).ConfigureAwait(false);
                        }

                        this.currentAction = action;
                        this.lastEvaluation = DateTime.Now.Ticks;
                    }
                }
                finally
                {
                    this.evaluationSemaphore.Release();
                }
            }

            if (this.currentAction is not null)
            {
                await this.currentAction.HandleAsync(context);
            }

            await next(context);
        }
    }
}

namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Extensions.ProducerThrottling.Configuration;

    internal class ProducerThrottlingMiddleware : IMessageMiddleware
    {
        private readonly EvaluationExecutor executor;

        public ProducerThrottlingMiddleware(ProducerEvaluationConfiguration evalConfig)
        {
            this.executor = new EvaluationExecutor(evalConfig);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            while (true)
            {
                var thresholdReached = await this.executor.Execute(async (currentValue, evalConfig) =>
                    {
                        foreach (var (threshold, action) in evalConfig.Actions.OrderByDescending(a => a.threshold))
                        {
                            if (currentValue > threshold)
                            {
                                await action.OnThresholdReachedHandler(
                                        context,
                                        currentValue,
                                        threshold)
                                    .ConfigureAwait(false);

                                return true;
                            }

                            await action.OnThresholdNotReachedHandler(
                                    context,
                                    currentValue,
                                    threshold)
                                .ConfigureAwait(false);
                        }

                        return false;
                    });

                if (!thresholdReached)
                {
                    break;
                }
            }

            await next(context);
        }

        private class EvaluationExecutor
        {
            private readonly Stopwatch timer;
            private readonly ProducerEvaluationConfiguration evalConfiguration;

            public EvaluationExecutor(ProducerEvaluationConfiguration evalConfiguration)
            {
                this.evalConfiguration = evalConfiguration;
                this.timer = Stopwatch.StartNew();
            }

            public async Task<bool> Execute(Func<long, ProducerEvaluationConfiguration, Task<bool>> handler)
            {
                if (this.timer.Elapsed < this.evalConfiguration.Frequency)
                {
                    return false;
                }

                this.timer.Restart();
                var currentValue = await this.evalConfiguration.Evaluation.Eval().ConfigureAwait(false);
                var delay = Task.Delay(this.evalConfiguration.Frequency);
                var thresholdReached = await handler(currentValue, this.evalConfiguration);

                if (thresholdReached)
                {
                    await delay.ConfigureAwait(false);
                }

                return thresholdReached;
            }
        }
    }
}

namespace KafkaFlow.Producers.Middlewares.Throttling.Actions
{
    using System;
    using System.Threading.Tasks;

    internal class DelayAction : IAction
    {
        private readonly TimeSpan delay;

        public DelayAction(int threshold, TimeSpan delay)
        {
            this.Threshold = threshold;
            this.delay = delay;
        }

        public int Threshold { get; }

        public Task HandleAsync(IMessageContext context) => Task.Delay(this.delay, context.ProducerContext.ClientStopped);

        public Task OnEndAsync(IMessageContext context) => Task.CompletedTask;
    }
}

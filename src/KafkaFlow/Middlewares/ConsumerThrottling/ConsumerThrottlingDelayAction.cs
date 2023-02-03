namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System;
    using System.Threading.Tasks;

    internal class ConsumerThrottlingDelayAction : IConsumerThrottlingAction
    {
        private readonly TimeSpan delay;

        public ConsumerThrottlingDelayAction(TimeSpan delay)
        {
            this.delay = delay;
        }

        public Task ExecuteAsync()
        {
            return Task.Delay(this.delay);
        }
    }
}

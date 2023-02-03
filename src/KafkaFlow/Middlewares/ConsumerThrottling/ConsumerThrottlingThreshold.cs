namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System.Threading.Tasks;

    internal class ConsumerThrottlingThreshold : IConsumerThrottlingThreshold
    {
        private readonly IConsumerThrottlingAction action;

        public ConsumerThrottlingThreshold(long thresholdValue, IConsumerThrottlingAction action)
        {
            this.action = action;
            this.ThresholdValue = thresholdValue;
        }

        public long ThresholdValue { get; }

        public async Task<bool> TryExecuteActionAsync(long metricValue)
        {
            if (this.ThresholdValue > metricValue)
            {
                return false;
            }

            await this.action.ExecuteAsync();

            return true;
        }
    }
}

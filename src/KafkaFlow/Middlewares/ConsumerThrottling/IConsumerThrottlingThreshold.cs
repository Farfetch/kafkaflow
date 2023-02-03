namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System.Threading.Tasks;

    internal interface IConsumerThrottlingThreshold
    {
        long ThresholdValue { get; }

        Task<bool> TryExecuteActionAsync(long metricValue);
    }
}

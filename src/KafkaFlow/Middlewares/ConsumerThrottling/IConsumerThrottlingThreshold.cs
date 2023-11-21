using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    internal interface IConsumerThrottlingThreshold
    {
        long ThresholdValue { get; }

        Task<bool> TryExecuteActionAsync(long metricValue);
    }
}

using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.ConsumerThrottling;

internal class ConsumerThrottlingThreshold : IConsumerThrottlingThreshold
{
    private readonly IConsumerThrottlingAction _action;

    public ConsumerThrottlingThreshold(long thresholdValue, IConsumerThrottlingAction action)
    {
        _action = action;
        this.ThresholdValue = thresholdValue;
    }

    public long ThresholdValue { get; }

    public async Task<bool> TryExecuteActionAsync(long metricValue)
    {
        if (this.ThresholdValue > metricValue)
        {
            return false;
        }

        await _action.ExecuteAsync();

        return true;
    }
}

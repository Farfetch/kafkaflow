using System;
using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.ConsumerThrottling;

internal class ConsumerThrottlingDelayAction : IConsumerThrottlingAction
{
    private readonly TimeSpan _delay;

    public ConsumerThrottlingDelayAction(TimeSpan delay)
    {
        _delay = delay;
    }

    public Task ExecuteAsync()
    {
        return Task.Delay(_delay);
    }
}

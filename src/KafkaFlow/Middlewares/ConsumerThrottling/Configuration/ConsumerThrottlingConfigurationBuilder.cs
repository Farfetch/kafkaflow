using System;
using System.Collections.Generic;
using System.Linq;

namespace KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

internal class ConsumerThrottlingConfigurationBuilder
    : IConsumerThrottlingMetricConfigurationBuilder,
        IConsumerThrottlingActionsConfigurationBuilder
{
    private readonly List<Func<IDependencyResolver, IConsumerThrottlingMetric>> _metrics = new();
    private readonly List<ConsumerThrottlingActionConfigurationBuilder> _actions = new();
    private TimeSpan _interval = TimeSpan.FromSeconds(5);

    public IConsumerThrottlingActionsConfigurationBuilder WithInterval(TimeSpan interval)
    {
        _interval = interval;
        return this;
    }

    public IConsumerThrottlingMetricConfigurationBuilder AddMetric(Func<IDependencyResolver, IConsumerThrottlingMetric> factory)
    {
        _metrics.Add(factory);
        return this;
    }

    public IConsumerThrottlingActionsConfigurationBuilder AddAction(Action<IConsumerThrottlingActionConfigurationBuilder> action)
    {
        var builder = new ConsumerThrottlingActionConfigurationBuilder();

        action(builder);

        _actions.Add(builder);
        return this;
    }

    public ConsumerThrottlingConfiguration Build(IDependencyResolver resolver)
    {
        return new ConsumerThrottlingConfiguration(
            _interval,
            _metrics
                .Select(x => x(resolver))
                .ToList(),
            _actions
                .Select(x => new ConsumerThrottlingThreshold(x.Threshold, x.Factory(resolver)))
                .ToList());
    }
}

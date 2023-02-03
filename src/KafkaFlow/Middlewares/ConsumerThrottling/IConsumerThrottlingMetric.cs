namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a metric that is used by the KafkaFlow consumer throttling feature.
    /// </summary>
    public interface IConsumerThrottlingMetric
    {
        /// <summary>
        /// Retrieves the value of the metric defined in the implementation of this interface.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with a <see cref="long"/> result representing the metric's value.</returns>
        Task<long> GetValueAsync();
    }
}

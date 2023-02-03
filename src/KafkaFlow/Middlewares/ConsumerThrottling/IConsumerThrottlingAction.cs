namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a throttling action that can be executed by a KafkaFlow consumer.
    /// </summary>
    public interface IConsumerThrottlingAction
    {
        /// <summary>
        /// Executes the action defined in the implementation of this interface.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExecuteAsync();
    }
}

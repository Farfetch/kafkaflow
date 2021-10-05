namespace KafkaFlow.Extensions.ProducerThrottling.Actions
{
    using System.Threading.Tasks;

    /// <summary>
    /// An interface used to create actions executed when the threshold is reached (or nor)
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// The handler to be executed when the threshold is reached
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <param name="currentValue">The current metric value</param>
        /// <param name="threshold">The threshold value</param>
        /// <returns></returns>
        Task OnThresholdReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold);

        /// <summary>
        /// The handler to be executed when the threshold is not reached
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <param name="currentValue">The current metric value</param>
        /// <param name="threshold">The current threshold value</param>
        /// <returns></returns>
        Task OnThresholdNotReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold);
    }
}

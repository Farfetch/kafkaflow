namespace KafkaFlow.Producers.Middlewares.Throttling.Actions
{
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    /// <summary>
    /// An interface used to create actions to be executed when the threshold is reached
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the action threshold
        /// </summary>
        int Threshold { get; }

        /// <summary>
        /// The handler to be executed when the threshold is reached
        /// </summary>
        /// <param name="context">The <see cref="IMessageContext"/> containing the message and metadata</param>
        /// <returns></returns>
        Task HandleAsync(IMessageContext context);

        /// <summary>
        /// The handler to be executed when the action is ended
        /// </summary>
        /// <param name="producerConfiguration">The producer configuration</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task OnEndAsync(IProducerConfiguration producerConfiguration, CancellationToken cancellationToken);
    }
}

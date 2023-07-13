namespace KafkaFlow.Consumers
{
    /// <summary>
    /// Provides access to the current consumer worker context. This interface only returns values when inside a middleware with Worker lifetime; otherwise, it will return null.
    /// </summary>
    public interface IWorkerLifetimeContext
    {
        /// <summary>
        /// Gets the current worker in the context.
        /// </summary>
        IWorker Worker { get; }

        /// <summary>
        /// Gets the current consumer in the context.
        /// </summary>
        IConsumer Consumer { get; }
    }
}

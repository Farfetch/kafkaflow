namespace KafkaFlow
{
    /// <summary>
    /// Provides access to the current consumer worker context. This interface only returns values when inside a middleware with Worker lifetime; otherwise, it will return null.
    /// </summary>
    public interface IWorkerContext
    {
        /// <summary>
        /// Gets the current worker in the context.
        /// </summary>
        IWorker Worker { get; }
    }
}

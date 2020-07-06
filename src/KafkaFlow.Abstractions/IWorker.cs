namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Represents the interface of a internal worker
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Gets worker's id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// This handler is called immediately after a worker completes the consumption of a message
        /// </summary>
        /// <param name="handler"><see cref="Action"/> to be executed</param>
        void OnTaskCompleted(Action handler);
    }
}

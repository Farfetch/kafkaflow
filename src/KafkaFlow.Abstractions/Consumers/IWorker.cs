namespace KafkaFlow
{
    using System;
    using KafkaFlow.Observer;

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

        /// <summary>
        /// Gets the subject for worker stopping events where observers can subscribe to receive notifications.
        /// </summary>
        ISubject<WorkerStoppingSubject, VoidObject> WorkerStopping { get; }

        /// <summary>
        /// Gets the subject for worker stopped events where observers can subscribe to receive notifications.
        /// </summary>
        ISubject<WorkerStoppedSubject, VoidObject> WorkerStopped { get; }
    }
}

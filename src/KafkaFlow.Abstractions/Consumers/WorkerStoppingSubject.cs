namespace KafkaFlow
{
    using KafkaFlow.Observer;

    /// <summary>
    /// Represents a subject specific to worker stopping events where observers can subscribe to receive notifications.
    /// </summary>
    public class WorkerStoppingSubject : Subject<WorkerStoppingSubject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerStoppingSubject"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        public WorkerStoppingSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

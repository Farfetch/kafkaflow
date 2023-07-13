namespace KafkaFlow
{
    using KafkaFlow.Observer;

    /// <summary>
    /// Represents a subject specific to worker stopped events where observers can subscribe to receive notifications.
    /// </summary>
    public class WorkerStoppedSubject : Subject<WorkerStoppedSubject, VoidObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerStoppedSubject"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        public WorkerStoppedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

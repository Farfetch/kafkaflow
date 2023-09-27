namespace KafkaFlow.Consumers
{
    using System;
    using KafkaFlow.Observer;

    public class WorkerErrorSubject : Subject<WorkerErrorSubject, Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerErrorSubject"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        public WorkerErrorSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

namespace KafkaFlow.Consumers
{
    using KafkaFlow.Observer;

    public class WorkerStartedSubject : Subject<WorkerStartedSubject, IMessageContext>
    {
        public WorkerStartedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}
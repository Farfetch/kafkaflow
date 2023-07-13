namespace KafkaFlow.Consumers
{
    using KafkaFlow.Observer;

    internal class WorkerPoolStoppedSubject : Subject<WorkerPoolStoppedSubject, VoidObject>
    {
        public WorkerPoolStoppedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

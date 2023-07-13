namespace KafkaFlow.Consumers
{
    using KafkaFlow.Observer;

    internal class WorkerPoolStoppedSubject : Subject<WorkerPoolStoppedSubject>
    {
        public WorkerPoolStoppedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

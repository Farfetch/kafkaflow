namespace KafkaFlow.Consumers
{
    using System;
    using KafkaFlow.Observer;

    public interface IConsumerInstrumentationObservers :
        ISubjectObserver<WorkerStartedSubject, IMessageContext>,
        ISubjectObserver<WorkerStoppedSubject, VoidObject>,
        ISubjectObserver<WorkerErrorSubject, Exception>
    {
    }
}

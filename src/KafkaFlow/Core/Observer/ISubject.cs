namespace KafkaFlow.Core.Observer
{
    internal interface ISubject<T>
        where T : ISubject<T>
    {
        void Subscribe(ISubjectObserver<T> observer);
    }
}

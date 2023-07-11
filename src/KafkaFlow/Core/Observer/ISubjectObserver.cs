namespace KafkaFlow.Core.Observer
{
    internal interface ISubjectObserver<T>
        where T : ISubject<T>
    {
        void OnNotification();
    }
}

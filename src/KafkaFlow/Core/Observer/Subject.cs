namespace KafkaFlow.Core.Observer
{
    using System.Collections.Generic;

    internal abstract class Subject<T> : ISubject<T>
        where T : ISubject<T>
    {
        private readonly List<ISubjectObserver<T>> observers = new();

        public void Subscribe(ISubjectObserver<T> observer) => this.observers.Add(observer);

        public void Notify()
        {
            foreach (var observer in this.observers)
            {
                observer.OnNotification();
            }
        }
    }
}

namespace KafkaFlow.Observer
{
    /// <summary>
    /// Represents a subject in the observer design pattern that can be observed by observers.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    public interface ISubject<T>
        where T : ISubject<T>
    {
        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        void Subscribe(ISubjectObserver<T> observer);
    }
}

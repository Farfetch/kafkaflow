namespace KafkaFlow.Observer
{
    /// <summary>
    /// Represents a subject in the observer design pattern that can be observed by observers.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TArg">An argument type that will be passed to the observers</typeparam>
    public interface ISubject<TSubject, TArg>
        where TSubject : Subject<TSubject, TArg>
    {
        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        void Subscribe(ISubjectObserver<TSubject, TArg> observer);
    }
}

namespace KafkaFlow.Observer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an observer in the observer design pattern that can receive notifications from a subject.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TArg">An argument type that will be passed to the observers</typeparam>
    public interface ISubjectObserver<in TSubject, in TArg>
    {
        /// <summary>
        /// Called when a notification is received from the subject.
        /// </summary>
        /// <returns>A task representing the asynchronous notification handling.</returns>
        Task OnNotification(TSubject subject, TArg arg);
    }
}

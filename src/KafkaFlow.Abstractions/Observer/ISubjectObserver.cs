namespace KafkaFlow.Observer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an observer in the observer design pattern that can receive notifications from a subject.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    public interface ISubjectObserver<T>
        where T : ISubject<T>
    {
        /// <summary>
        /// Called when a notification is received from the subject.
        /// </summary>
        /// <returns>A task representing the asynchronous notification handling.</returns>
        Task OnNotification();
    }
}

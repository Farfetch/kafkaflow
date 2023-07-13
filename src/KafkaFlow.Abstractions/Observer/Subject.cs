namespace KafkaFlow.Observer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a subject in the observer design pattern that can be observed by multiple observers.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    public abstract class Subject<T> : ISubject<T>
        where T : ISubject<T>
    {
        private readonly ILogHandler logHandler;
        private readonly List<ISubjectObserver<T>> observers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Subject{T}"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        protected Subject(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
        }

        /// <summary>
        /// Subscribes an observer to the subject, allowing it to receive notifications.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        public void Subscribe(ISubjectObserver<T> observer) => this.observers.Add(observer);

        /// <summary>
        /// Notifies all subscribed observers asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous notification operation.</returns>
        public async Task NotifyAsync()
        {
            foreach (var observer in this.observers)
            {
                try
                {
                    await observer.OnNotification();
                }
                catch (Exception e)
                {
                    this.logHandler.Error("Error notifying observer", e, new { Subject = typeof(T).Name });
                }
            }
        }
    }
}

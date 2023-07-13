namespace KafkaFlow.Observer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A generic implementation that should be extended to help the use of the notification system.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TArg">An argument type that will be passed to the observers</typeparam>
    public abstract class Subject<TSubject, TArg> : ISubject<TSubject, TArg>
        where TSubject : Subject<TSubject, TArg>
    {
        private readonly ILogHandler logHandler;
        private readonly List<ISubjectObserver<TSubject, TArg>> observers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Subject{TSubject, TArg}"/> class.
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
        public void Subscribe(ISubjectObserver<TSubject, TArg> observer) => this.observers.Add(observer);

        /// <summary>
        /// Notifies all subscribed observers asynchronously.
        /// </summary>
        /// <param name="arg">The parameter passed by the client.</param>
        /// <returns>A task representing the asynchronous notification operation.</returns>
        public async Task NotifyAsync(TArg arg)
        {
            foreach (var observer in this.observers)
            {
                try
                {
                    await observer.OnNotification((TSubject)this, arg);
                }
                catch (Exception e)
                {
                    this.logHandler.Error("Error notifying observer", e, new { Subject = this.GetType().Name });
                }
            }
        }
    }
}

namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly ILogHandler logHandler;

        private readonly List<Func<TArg, Task>> handlers = new();

        public Event(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
        }

        public IEventSubscription Subscribe(Func<TArg, Task> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException("Handler cannot be null");
            }

            if (!this.handlers.Contains(handler))
            {
                this.handlers.Add(handler);
            }

            return new EventSubscription(() => this.handlers.Remove(handler));
        }

        internal Task FireAsync(TArg arg)
        {
            var tasks = this.handlers
                .Select(handler => this.ProcessHandler(handler, arg));

            return Task.WhenAll(tasks);
        }

        private Task ProcessHandler(Func<TArg, Task> handler, TArg arg)
        {
            try
            {
                return handler.Invoke(arg).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        this.LogHandlerOnError(t.Exception);
                    }
                });
            }
            catch (Exception ex)
            {
                this.LogHandlerOnError(ex);
                return Task.CompletedTask;
            }
        }

        private void LogHandlerOnError(Exception ex)
        {
            this.logHandler.Error("Error firing event", ex, new { Event = this.GetType().Name });
        }
    }

    internal class Event : IEvent
    {
        private readonly Event<object> evt;

        public Event(ILogHandler logHandler)
        {
            this.evt = new Event<object>(logHandler);
        }

        public IEventSubscription Subscribe(Func<Task> handler) => this.evt.Subscribe(_ => handler.Invoke());

        internal Task FireAsync() => this.evt.FireAsync(null);
    }
}
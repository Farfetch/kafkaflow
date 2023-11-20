namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly ILogHandler logHandler;

        private readonly List<Func<TArg, Task>> asyncHandlers = new();
        private readonly List<Action<TArg>> syncHandlers = new();

        public Event(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
        }

        public IEventSubscription Subscribe(Func<TArg, Task> handler)
        {
            if (!this.asyncHandlers.Contains(handler))
            {
                this.asyncHandlers.Add(handler);
            }

            return new EventSubscription(() => this.asyncHandlers.Remove(handler));
        }

        internal Task FireAsync(TArg arg)
        {
            return Task.WhenAll(this.FireAsyncCall(arg));
        }

        private IEnumerable<Task> FireAsyncCall(TArg arg)
        {
            foreach (var handler in this.asyncHandlers)
            {
                    if (handler is null)
                    {
                        continue;
                    }

                    yield return handler.Invoke(arg).ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            this.logHandler.Error("Error firing event", t.Exception, new { Event = this.GetType().Name });
                        }
                    });
            }
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
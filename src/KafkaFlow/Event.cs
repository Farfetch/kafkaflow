namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly ILogHandler logHandler;

        private readonly IList<Func<TArg, Task>> handlers = new List<Func<TArg, Task>>();

        public Event(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
        }

        public IEventSubscription Subscribe(Func<TArg, Task> handler)
        {
            if (!this.handlers.Contains(handler))
            {
                this.handlers.Add(handler);
            }

            return new EventSubscription(() => this.handlers.Remove(handler));
        }

        internal async Task FireAsync(TArg arg)
        {
            foreach (var handler in this.handlers)
            {
                try
                {
                    if (handler is null)
                    {
                        continue;
                    }

                    await handler.Invoke(arg);
                }
                catch (Exception e)
                {
                    this.logHandler.Error("Error firing event", e, new { Event = this.GetType().Name });
                }
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

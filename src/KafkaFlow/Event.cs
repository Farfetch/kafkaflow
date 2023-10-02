namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly ILogHandler logHandler;

        private readonly IList<Func<TArg, Task>> handlers;

        public Event(ILogHandler logHandler, IEnumerable<Func<TArg, Task>> handlers)
        {
            this.logHandler = logHandler;
            this.handlers = handlers.ToList();
        }

        public Event(ILogHandler logHandler)
            : this(logHandler, Enumerable.Empty<Func<TArg, Task>>())
        {
        }

        public void Subscribe(Func<TArg, Task> handler)
        {
            this.handlers.Add(handler);
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

        public void Subscribe(Func<Task> handle)
        {
            this.evt.Subscribe(_ => handle?.Invoke());
        }

        internal Task FireAsync()
        {
            return this.evt.FireAsync(null);
        }
    }
}

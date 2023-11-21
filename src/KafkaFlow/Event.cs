using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaFlow
{
    internal class Event<TArg> : IEvent<TArg>
    {
        private readonly ILogHandler _logHandler;

        private readonly List<Func<TArg, Task>> _handlers = new();

        public Event(ILogHandler logHandler)
        {
            _logHandler = logHandler;
        }

        public IEventSubscription Subscribe(Func<TArg, Task> handler)
        {
            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }

            return new EventSubscription(() => _handlers.Remove(handler));
        }

        internal async Task FireAsync(TArg arg)
        {
            foreach (var handler in _handlers)
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
                    _logHandler.Error("Error firing event", e, new { Event = this.GetType().Name });
                }
            }
        }
    }

    internal class Event : IEvent
    {
        private readonly Event<object> _evt;

        public Event(ILogHandler logHandler)
        {
            _evt = new Event<object>(logHandler);
        }

        public IEventSubscription Subscribe(Func<Task> handler) => _evt.Subscribe(_ => handler.Invoke());

        internal Task FireAsync() => _evt.FireAsync(null);
    }
}

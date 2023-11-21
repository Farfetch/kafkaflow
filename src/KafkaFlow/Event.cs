using System;
using System.Collections.Generic;
using System.Linq;
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
            if (handler is null)
            {
                throw new ArgumentNullException("Handler cannot be null");
            }

            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }

            return new EventSubscription(() => _handlers.Remove(handler));
        }

        internal Task FireAsync(TArg arg)
        {
            var tasks = _handlers
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
            _logHandler.Error("Error firing event", ex, new { Event = this.GetType().Name });
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
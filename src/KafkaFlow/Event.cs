namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            return this.Subscribe(this.asyncHandlers, handler);
        }

        public IEventSubscription Subscribe(Action<TArg> handler)
        {
            return this.Subscribe(this.syncHandlers, handler);
        }

        internal Task FireAsync(TArg arg)
        {
            return this.InvokeAsyncHandlers(arg);
        }

        internal void Fire(TArg arg)
        {
            this.InvokeSyncHandlers(arg);
        }

        private IEventSubscription Subscribe<T>(List<T> handlersList, T handler)
        {
            if (!handlersList.Contains(handler))
            {
                handlersList.Add(handler);
            }

            return new EventSubscription(() => handlersList.Remove(handler));
        }

        private async Task InvokeAsyncHandlers(TArg arg)
        {
            foreach (var handler in this.asyncHandlers.Where(h => h is not null))
            {
                try
                {
                    await handler.Invoke(arg);
                }
                catch (Exception ex)
                {
                    this.LogHandlerOnError(ex);
                }
            }
        }

        private void InvokeSyncHandlers(TArg arg)
        {
            foreach (var handler in this.syncHandlers.Where(h => h is not null))
            {
                try
                {
                    handler.Invoke(arg);
                }
                catch (Exception ex)
                {
                    this.LogHandlerOnError(ex);
                }
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

        public IEventSubscription Subscribe(Action handler) => this.evt.Subscribe(_ => handler.Invoke());

        internal Task FireAsync() => this.evt.FireAsync(null);
    }
}
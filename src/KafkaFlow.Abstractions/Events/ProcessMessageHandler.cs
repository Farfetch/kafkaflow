using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Events
{
    public sealed class ProcessMessageHandler : IProcessMessageHandler //IObservable<IMessageContext>
    {
        private readonly HashSet<IObserver<IMessageContext>> observers = new();

        public IDisposable Subscribe(IObserver<IMessageContext> observer)
        {
            this.observers.Add(observer);

            return new Unsubscriber<IMessageContext>(this.observers, observer);
        }

        public HashSet<IObserver<IMessageContext>> GetObservers() => this.observers;
    }

    public interface IProcessMessageHandler : IObservable<IMessageContext>
    {
        HashSet<IObserver<IMessageContext>> GetObservers();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Events
{
    internal sealed class Unsubscriber<IMessageContext> : IDisposable
    {
        private readonly ISet<IObserver<IMessageContext>> _observers;
        private readonly IObserver<IMessageContext> _observer;

        internal Unsubscriber(
            ISet<IObserver<IMessageContext>> observers,
            IObserver<IMessageContext> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }
}

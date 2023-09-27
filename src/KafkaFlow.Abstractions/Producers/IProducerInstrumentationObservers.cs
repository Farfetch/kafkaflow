using System;
using System.Collections.Generic;
using System.Text;
using KafkaFlow.Observer;

namespace KafkaFlow.Producers
{
    public interface IProducerInstrumentationObservers :
        ISubjectObserver<ProducerStartedSubject, IMessageContext>,
        ISubjectObserver<ProducerStoppedSubject, VoidObject>,
        ISubjectObserver<ProducerErrorSubject, Exception>
    {
    }
}

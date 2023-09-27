namespace KafkaFlow.Producers
{
    using KafkaFlow;
    using KafkaFlow.Observer;

    public class ProducerStartedSubject : Subject<ProducerStartedSubject, IMessageContext>
    {
        public ProducerStartedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

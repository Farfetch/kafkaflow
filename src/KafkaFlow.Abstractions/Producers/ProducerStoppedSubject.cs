namespace KafkaFlow.Producers
{
    using KafkaFlow.Observer;

    public class ProducerStoppedSubject : Subject<ProducerStoppedSubject, VoidObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerStoppedSubject"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        public ProducerStoppedSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}
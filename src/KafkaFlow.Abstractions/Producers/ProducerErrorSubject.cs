namespace KafkaFlow.Producers
{
    using System;
    using KafkaFlow.Observer;

    public class ProducerErrorSubject : Subject<ProducerErrorSubject, Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerErrorSubject"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
        public ProducerErrorSubject(ILogHandler logHandler)
            : base(logHandler)
        {
        }
    }
}

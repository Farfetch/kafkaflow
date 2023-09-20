using System;

namespace KafkaFlow.Events.Args
{
    public class ProduceErrorEventArgs
    {
        public ProduceErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; set; }
    }
}
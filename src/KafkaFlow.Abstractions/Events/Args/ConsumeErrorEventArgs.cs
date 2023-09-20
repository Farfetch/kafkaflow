using System;

namespace KafkaFlow.Events.Args
{
    public class ConsumeErrorEventArgs
    {
        public ConsumeErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; set; }
    }
}
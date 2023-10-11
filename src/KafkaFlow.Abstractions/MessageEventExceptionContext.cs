namespace KafkaFlow
{
    using System;

    public class MessageEventExceptionContext
    {
        private readonly IMessageContext messageContext;
        private readonly Exception exception;

        public MessageEventExceptionContext(IMessageContext messageContext, Exception exception)
        {
            this.messageContext = messageContext;
            this.exception = exception;

        }

        public IMessageContext MessageContext => this.messageContext;

        public Exception Exception => this.exception;
    }
}

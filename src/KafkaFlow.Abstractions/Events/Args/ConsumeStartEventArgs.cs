namespace KafkaFlow.Events.Args
{
    public class ConsumeStartEventArgs
    {
        internal ConsumeStartEventArgs(IMessageContext context)
        {
            this.MessageContext = context;
        }

        public IMessageContext MessageContext { get; set; }
    }
}
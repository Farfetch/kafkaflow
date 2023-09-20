namespace KafkaFlow.Events.Args
{
    public class ProduceStartEventArgs
    {
        internal ProduceStartEventArgs(IMessageContext context)
        {
            this.MessageContext = context;
        }

        public IMessageContext MessageContext { get; set; }
    }
}
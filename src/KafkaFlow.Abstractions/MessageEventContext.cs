namespace KafkaFlow
{
    public class MessageEventContext
    {
        public IMessageContext MessageContext { get; set; }

        public IDependencyResolver DependencyResolver { get; set; }
    }
}

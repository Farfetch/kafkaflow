namespace KafkaFlow.Abstractions
{
    public class MessageEventContext
    {
        public IMessageContext MessageContext { get; set; }

        public IDependencyResolver DependencyResolver { get; set; }
    }
}

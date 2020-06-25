namespace KafkaFlow.TypedHandler
{
    using System.Threading.Tasks;

    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        Task Handle(IMessageContext context, TMessage message);
    }

    public interface IMessageHandler
    {
    }
}

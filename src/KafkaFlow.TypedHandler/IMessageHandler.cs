namespace KafkaFlow.TypedHandler
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the interface to be implemented by custom message handlers
    /// </summary>
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// Handles a message of the <typeparamref name="TMessage"/> type
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="message"><typeparamref name="TMessage"/> message to be processed</param>
        /// <returns></returns>
        Task Handle(IMessageContext context, TMessage message);
    }

    /// <summary>
    /// Represents the interface to be implemented by custom message handlers
    /// </summary>
    public interface IMessageHandler
    {
    }
}

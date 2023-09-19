namespace KafkaFlow
{
    using System.Threading.Tasks;

    /// <summary>
    /// Used to create a message handler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// the method that will be called when a <typeparamref name="TMessage"/> arrives
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="message"><typeparamref name="TMessage"/>The message type to be processed</param>
        /// <returns></returns>
        Task Handle(IMessageContext context, TMessage message);
    }

    /// <summary>
    /// Used to create a message handler
    /// </summary>
    public interface IMessageHandler
    {
    }
}

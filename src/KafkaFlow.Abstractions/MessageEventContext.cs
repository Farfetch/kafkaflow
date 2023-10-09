namespace KafkaFlow
{
    /// <summary>
    /// Represents a message context used in the events
    /// </summary>
    public class MessageEventContext
    {
        private readonly IMessageContext messageContext;
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        /// <param name="dependencyResolver">The dependency resolver</param>
        public MessageEventContext(IMessageContext messageContext, IDependencyResolver dependencyResolver)
        {
            this.messageContext = messageContext;
            this.dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventContext"/> class.
        /// </summary>
        /// <param name="messageContext">The message context</param>
        public MessageEventContext(IMessageContext messageContext)
            : this(messageContext, null)
        {
        }

        /// <summary>
        /// Gets the message context
        /// </summary>
        public IMessageContext MessageContext => this.messageContext;

        /// <summary>
        /// Gets the dependency resolver
        /// </summary>
        public IDependencyResolver DependencyResolver => this.dependencyResolver;
    }
}

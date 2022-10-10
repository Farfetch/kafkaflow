namespace KafkaFlow.Admin
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;

    /// <summary>
    /// A special producer to publish admin messages
    /// </summary>
    internal interface IAdminProducer
    {
        /// <summary>
        /// Produces admin messages to be listened by other application instances
        /// </summary>
        /// <param name="message">A message that implements the <see cref="IAdminMessage"/></param>
        /// <returns></returns>
        Task ProduceAsync(IAdminMessage message);
    }
}

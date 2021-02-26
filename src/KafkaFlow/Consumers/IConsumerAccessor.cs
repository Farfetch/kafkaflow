namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to the configured consumers
    /// </summary>
    public interface IConsumerAccessor
    {
        /// <summary>
        /// Gets a consumer by its name
        /// </summary>
        /// <param name="name">The name defined in the consumer configuration</param>
        /// <returns></returns>
        IMessageConsumer GetConsumer(string name);

        /// <summary>
        /// Returns all configured consumers
        /// </summary>
        IEnumerable<IMessageConsumer> All { get; }

        /// <summary>
        /// Gets a consumer by its name
        /// </summary>
        /// <param name="name"></param>
        IMessageConsumer this[string name] { get; }

        internal void Add(IMessageConsumer consumer);
    }
}

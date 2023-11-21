using System.Collections.Generic;

namespace KafkaFlow.Consumers
{
    /// <summary>
    /// Provides access to the configured consumers
    /// </summary>
    public interface IConsumerAccessor
    {
        /// <summary>
        /// Gets all configured consumers
        /// </summary>
        IEnumerable<IMessageConsumer> All { get; }

        /// <summary>
        /// Gets a consumer by its name
        /// </summary>
        /// <param name="name">consumer name</param>
        IMessageConsumer this[string name] { get; }

        /// <summary>
        /// Gets a consumer by its name
        /// </summary>
        /// <param name="name">The name defined in the consumer configuration</param>
        /// <returns></returns>
        IMessageConsumer GetConsumer(string name);

        internal void Add(IMessageConsumer consumer);
    }
}

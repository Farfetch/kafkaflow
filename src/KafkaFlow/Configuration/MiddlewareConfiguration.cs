namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the middleware configurations
    /// </summary>
    public class MiddlewareConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiddlewareConfiguration"/> class.
        /// </summary>
        /// <param name="factories">A collection of middleware factories</param>
        public MiddlewareConfiguration(IReadOnlyList<Factory<IMessageMiddleware>> factories)
        {
            this.Factories = factories;
        }

        /// <summary>
        /// Gets the collection of middleware factories
        /// </summary>
        public IReadOnlyList<Factory<IMessageMiddleware>> Factories { get; }
    }
}

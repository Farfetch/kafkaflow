namespace KafkaFlow.Client.Protocol.Messages
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a broker capabilities entity
    /// </summary>
    public class BrokerCapabilities : IBrokerCapabilities
    {
        private readonly Dictionary<ApiKey, ApiVersionRange> ranges;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerCapabilities"/> class.
        /// </summary>
        /// <param name="ranges">The range of api versions support by the broker</param>
        public BrokerCapabilities(IEnumerable<ApiVersionRange> ranges)
        {
            this.ranges = ranges.ToDictionary(x => x.Api);
        }

        /// <inheritdoc/>
        public ApiVersionRange GetVersionRange(ApiKey api)
        {
            return this.ranges[api];
        }
    }
}

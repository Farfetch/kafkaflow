namespace KafkaFlow.Client.Messages.Adapters
{
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Protocol;

    internal class HostCapabilities : IHostCapabilities
    {
        private readonly Dictionary<ApiKey, ApiVersionRange> ranges;

        public HostCapabilities(IEnumerable<ApiVersionRange> ranges)
        {
            this.ranges = ranges.ToDictionary(x => x.Api);
        }

        public ApiVersionRange GetVersionRange(ApiKey api)
        {
            return this.ranges[api];
        }
    }
}

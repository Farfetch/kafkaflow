namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Represents the range of versions support by the broker to an api operation
    /// </summary>
    public readonly struct ApiVersionRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionRange"/> struct.
        /// </summary>
        /// <param name="api">The operation key</param>
        /// <param name="min">The min version</param>
        /// <param name="max">The max version</param>
        public ApiVersionRange(ApiKey api, short min, short max)
        {
            this.Api = api;
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Gets the operation key
        /// </summary>
        public ApiKey Api { get; }

        /// <summary>
        /// Gets the min version supported
        /// </summary>
        public short Min { get; }

        /// <summary>
        /// Gets the max version supported
        /// </summary>
        public short Max { get; }
    }
}

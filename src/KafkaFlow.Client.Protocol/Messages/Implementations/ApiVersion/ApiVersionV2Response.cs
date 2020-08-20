namespace KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion
{
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Response message with the full list of supported api keys and versions by the broker
    /// </summary>
    public class ApiVersionV2Response : IApiVersionResponse
    {
        /// <inheritdoc/>
        public ErrorCode Error { get; private set; }

        /// <inheritdoc/>
        public IApiVersionResponse.IApiVersion[] ApiVersions { get; private set; }

        /// <inheritdoc/>
        public int ThrottleTime { get; private set; }

        /// <inheritdoc/>
        void IResponse.Read(MemoryReader source)
        {
            this.Error = source.ReadErrorCode();
            this.ApiVersions = source.ReadArray<ApiVersion>();
            this.ThrottleTime = source.ReadInt32();
        }

        /// <summary>
        /// Represents an api version supported by the broker
        /// </summary>
        private class ApiVersion : IApiVersionResponse.IApiVersion
        {
            /// <inheritdoc/>
            public ApiKey ApiKey { get; private set; }

            /// <inheritdoc/>
            public short MinVersion { get; private set; }

            /// <inheritdoc/>
            public short MaxVersion { get; private set; }

            /// <inheritdoc/>
            void IResponse.Read(MemoryReader source)
            {
                this.ApiKey = (ApiKey) source.ReadInt16();
                this.MinVersion = source.ReadInt16();
                this.MaxVersion = source.ReadInt16();
            }
        }
    }
}

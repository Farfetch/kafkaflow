namespace KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Request message to achieve the full list of supported api keys and versions by the broker
    /// </summary>
    public class ApiVersionV2Request : IApiVersionRequest
    {
        /// <inheritdoc/>
        public ApiKey ApiKey => ApiKey.ApiVersions;

        /// <inheritdoc/>
        public short ApiVersion => 2;

        /// <inheritdoc/>
        public Type ResponseType => typeof(ApiVersionV2Response);

        /// <inheritdoc/>
        void IRequest.Write(MemoryWriter destination)
        {
            // Do Nothing
        }
    }
}

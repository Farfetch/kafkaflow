namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create ApiVersion responses
    /// </summary>
    public interface IApiVersionResponse : IResponse
    {
        /// <summary>
        /// Used to create the an api version supported by the broker
        /// </summary>
        public interface IApiVersion : IResponse
        {
            /// <summary>
            /// Gets the API index
            /// </summary>
            ApiKey ApiKey { get; }

            /// <summary>
            /// Gets the minimum supported version
            /// </summary>
            short MinVersion { get; }

            /// <summary>
            /// Gets the maximum supported version
            /// </summary>
            short MaxVersion { get; }
        }

        /// <summary>
        /// Gets the top-level error code
        /// </summary>
        ErrorCode Error { get; }

        /// <summary>
        /// Gets the API versions supported by the broker
        /// </summary>
        IApiVersion[] ApiVersions { get; }

        /// <summary>
        /// Gets the duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota
        /// </summary>
        int ThrottleTime { get; }
    }
}

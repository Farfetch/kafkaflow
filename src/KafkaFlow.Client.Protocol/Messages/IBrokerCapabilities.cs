namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    /// Used to create a broker capabilities entity
    /// </summary>
    public interface IBrokerCapabilities
    {
        /// <summary>
        /// Gets the range of versions support by the broker of a specific api operation
        /// </summary>
        /// <param name="api">The api key operation</param>
        /// <returns></returns>
        ApiVersionRange GetVersionRange(ApiKey api);
    }
}

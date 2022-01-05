namespace KafkaFlow.Client.Protocol
{
    using System;

    /// <summary>
    /// Used to create request messages with response type associated
    /// </summary>
    /// <typeparam name="TResponse">A class that implements the <see cref="IResponse"/> interface</typeparam>
    public interface IRequestMessage<TResponse> : IRequestMessage
        where TResponse : IResponse
    {
        /// <summary>
        /// Gets the response type expected when the request is sent
        /// </summary>
        public Type ResponseType { get; }
    }

    /// <summary>
    /// Used to create request messages
    /// </summary>
    public interface IRequestMessage : IRequest
    {
        /// <summary>
        /// Gets the api key of the request
        /// </summary>
        public ApiKey ApiKey { get; }

        /// <summary>
        /// Gets the version of the request
        /// </summary>
        public short ApiVersion { get; }
    }
}

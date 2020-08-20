namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to create broker connection class
    /// </summary>
    public interface IBrokerConnection : IAsyncDisposable
    {
        /// <summary>
        /// Gets the broker address
        /// </summary>
        BrokerAddress Address { get; }

        /// <summary>
        /// Send a message to the broker asynchronously
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <typeparam name="TResponse">A class that implements the <see cref="IResponse"/> interface</typeparam>
        /// <returns></returns>
        Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> message)
            where TResponse : class, IResponse;
    }
}

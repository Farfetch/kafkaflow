namespace KafkaFlow.Client.Protocol
{
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Used to create request messages
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Write data from request message into MemoryWriter
        /// </summary>
        /// <param name="destination">The <see cref="MemoryWriter"/> instance</param>
        internal void Write(MemoryWriter destination);
    }
}

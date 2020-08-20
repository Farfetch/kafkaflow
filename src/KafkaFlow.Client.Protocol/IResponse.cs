namespace KafkaFlow.Client.Protocol
{
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Used to create response messages
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Read data from MemoryReader and set into response message
        /// </summary>
        /// <param name="source">The <see cref="MemoryReader"/> instance</param>
        internal void Read(MemoryReader source);
    }
}

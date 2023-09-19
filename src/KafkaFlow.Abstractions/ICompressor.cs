namespace KafkaFlow
{
    /// <summary>
    /// Used to create a message compressor
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compress the given message
        /// </summary>
        /// <param name="message">The message to be compressed</param>
        /// <returns>The compressed message</returns>
        byte[] Compress(byte[] message);
    }
}

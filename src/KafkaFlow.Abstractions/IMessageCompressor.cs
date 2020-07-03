namespace KafkaFlow
{
    /// <summary>
    /// Represents the interface to be implemented by custom message compressors
    /// </summary>
    public interface IMessageCompressor
    {
        /// <summary>
        /// Compress the given message
        /// </summary>
        /// <param name="message">The message to be compressed</param>
        /// <returns>The compressed message</returns>
        byte[] Compress(byte[] message);

        /// <summary>
        /// Decompress the given message
        /// </summary>
        /// <param name="message">The message to be decompressed</param>
        /// <returns>The decompressed message</returns>
        byte[] Decompress(byte[] message);
    }
}

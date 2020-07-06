namespace KafkaFlow
{
    /// <summary>
    /// Used to create a message compressor
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

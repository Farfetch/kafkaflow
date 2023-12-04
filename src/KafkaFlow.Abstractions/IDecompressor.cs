namespace KafkaFlow;

/// <summary>
/// Used to create a message decompressor
/// </summary>
public interface IDecompressor
{
    /// <summary>
    /// Decompress the given message
    /// </summary>
    /// <param name="message">The message to be decompressed</param>
    /// <returns>The decompressed message</returns>
    byte[] Decompress(byte[] message);
}

namespace KafkaFlow.Compressor.Gzip
{
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Compressor class for messages using gzip format
    /// </summary>    
    public class GzipMessageCompressor : IMessageCompressor
    {
        /// <summary>Compress the given message into gzip format</summary>
        /// <param name="message">The message to be compressed</param>
        /// <returns>Message compressed in gzip format</returns>
        public byte[] Compress(byte[] message)
        {
            using var inputStream = new MemoryStream(message);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal);
            inputStream.CopyTo(gzipStream);

            return outputStream.ToArray();
        }

        /// <summary>Decompress the given compressed message</summary>
        /// <param name="message">The message to be decompressed</param>
        /// <returns>Message decompressed from gzip format</returns>
        public byte[] Decompress(byte[] message)
        {
            using var outputStream = new MemoryStream();
            using var inputStream = new MemoryStream(message);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            gzipStream.CopyTo(outputStream);

            return outputStream.ToArray();
        }
    }
}

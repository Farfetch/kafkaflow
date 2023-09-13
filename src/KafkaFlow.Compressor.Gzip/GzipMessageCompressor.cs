namespace KafkaFlow.Compressor.Gzip
{
    using System;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// A GZIP message compressor
    /// </summary>
    public class GzipMessageCompressor : IMessageCompressor
    {
        /// <inheritdoc />
        public byte[] Compress(byte[] message)
        {
            using var inputStream = new MemoryStream(message);
            using var outputStream = new MemoryStream();

            using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                inputStream.CopyTo(gzipStream);
            }

            return outputStream.ToArray();
        }

        /// <inheritdoc />
        public byte[] Decompress(byte[] message)
        {
            using var outputStream = new MemoryStream();
            using var inputStream = new MemoryStream(message);

            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }

            return outputStream.ToArray();
        }
    }
}

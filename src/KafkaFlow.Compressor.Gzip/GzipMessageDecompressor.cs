using System.IO;
using System.IO.Compression;

namespace KafkaFlow.Compressor.Gzip
{
    /// <summary>
    /// A GZIP message decompressor
    /// </summary>
    public class GzipMessageDecompressor : IDecompressor
    {
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

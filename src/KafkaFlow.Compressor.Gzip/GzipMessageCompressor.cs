using System.IO;
using System.IO.Compression;

namespace KafkaFlow.Compressor.Gzip
{
    /// <summary>
    /// A GZIP message compressor
    /// </summary>
    public class GzipMessageCompressor : ICompressor
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
    }
}

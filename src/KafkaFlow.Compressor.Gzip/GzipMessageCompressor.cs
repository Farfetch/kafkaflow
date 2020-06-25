namespace KafkaFlow.Compressor.Gzip
{
    using System.IO;
    using System.IO.Compression;

    public class GzipMessageCompressor : IMessageCompressor
    {
        public byte[] Compress(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
                {
                    inputStream.CopyTo(gzipStream);
                }

                return outputStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            using (var inputStream = new MemoryStream(data))
            {
                using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputStream);
                }

                return outputStream.ToArray();
            }
        }
    }
}

namespace KafkaFlow.Client.Protocol
{
    using System.IO;
    using Microsoft.IO;

    internal static class MemoryStreamFactory
    {
        private static readonly RecyclableMemoryStreamManager MemoryManager = new RecyclableMemoryStreamManager(
            1024 * 16,
            1024 * 16,
            1024 * 1024 * 1024,
            false)
        {
            AggressiveBufferReturn = true
        };

        public static MemoryStream GetStream() => MemoryManager.GetStream();
    }
}

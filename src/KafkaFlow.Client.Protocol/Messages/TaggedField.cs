namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Represents a tagged field
    /// </summary>
    public class TaggedField : IRequest, IResponse
    {
        private int Tag { get; set; }

        private byte[] Data { get;  set; } = Array.Empty<byte>();

        /// <inheritdoc/>
        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteUVarint((ulong) this.Tag);
            destination.WriteUVarint((ulong) this.Data.Length);
            destination.Write(this.Data);
        }

        /// <inheritdoc/>
        void IResponse.Read(MemoryReader source)
        {
            this.Tag = source.ReadUVarint();
            this.Data = source.GetSpan(source.ReadUVarint()).ToArray();
        }
    }
}

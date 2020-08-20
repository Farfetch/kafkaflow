namespace KafkaFlow.Client.Protocol.Messages
{
    using System;
    using System.IO;

    public class TaggedField : IRequest, IResponse
    {
        public int Tag { get; private set; }

        public byte[] Data { get; private set; } = Array.Empty<byte>();

        public void Write(Stream destination)
        {
            destination.WriteUVarint((ulong) this.Tag);
            destination.WriteUVarint((ulong) this.Data.Length);
            destination.Write(this.Data);
        }

        public void Read(Stream source)
        {
            this.Tag = source.ReadUVarint();
            this.Data = source.ReadBytes(source.ReadUVarint());
        }
    }
}

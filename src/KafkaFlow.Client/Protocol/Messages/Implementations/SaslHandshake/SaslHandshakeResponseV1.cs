namespace KafkaFlow.Client.Protocol.Messages.Implementations.SaslHandshake
{
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    public class SaslHandshakeResponseV1 : ISaslHandshakeResponse
    {
        public short ErrorCode { get; private set; }

        public string[] Mechanisms { get; private set; }

        public void Read(MemoryReader source)
        {
            this.ErrorCode = source.ReadInt16();

            this.Mechanisms = new string[source.ReadInt32()];
            for (int i = 0; i < this.Mechanisms.Length; i++)
            {
                this.Mechanisms[i] = source.ReadString();
            }
        }
    }
}

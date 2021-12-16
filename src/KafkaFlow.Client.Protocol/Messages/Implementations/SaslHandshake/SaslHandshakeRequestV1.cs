namespace KafkaFlow.Client.Protocol.Messages.Implementations.SaslHandshake
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    public class SaslHandshakeRequestV1 : ISaslHandshakeRequest
    {
        public SaslHandshakeRequestV1(string mechanism)
        {
            this.Mechanism = mechanism;
        }

        public ApiKey ApiKey => ApiKey.SaslHandshake;

        public short ApiVersion => 1;

        public Type ResponseType => typeof(SaslHandshakeResponseV1);

        public string Mechanism { get; }

        public void Write(MemoryWriter destination)
        {
            destination.WriteString(this.Mechanism);
        }
    }
}

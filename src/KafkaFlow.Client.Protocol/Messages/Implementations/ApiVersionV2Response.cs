namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.IO;
    using KafkaFlow.Client.Protocol.Streams;

    public class ApiVersionV2Response : IResponse
    {
        public ErrorCode Error { get; private set; }

        public ApiVersion[] ApiVersions { get; private set; }

        public int ThrottleTime { get; private set; }

        public void Read(Stream source)
        {
            this.Error = source.ReadErrorCode();
            this.ApiVersions = source.ReadArray<ApiVersion>();
            this.ThrottleTime = source.ReadInt32();
        }

        public class ApiVersion : IResponse
        {
            public ApiKey ApiKey { get; private set; }

            public short MinVersion { get; private set; }

            public short MaxVersion { get; private set; }

            public void Read(Stream source)
            {
                this.ApiKey = (ApiKey) source.ReadInt16();
                this.MinVersion = source.ReadInt16();
                this.MaxVersion = source.ReadInt16();
            }
        }
    }
}

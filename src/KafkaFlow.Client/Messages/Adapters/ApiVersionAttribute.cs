namespace KafkaFlow.Client.Messages.Adapters
{
    using System;

    internal class ApiVersionAttribute : Attribute
    {
        public int Version { get; }

        public ApiVersionAttribute(int version) => this.Version = version;
    }
}
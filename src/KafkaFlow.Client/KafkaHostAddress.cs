namespace KafkaFlow.Client
{
    using System;

    public class KafkaHostAddress
    {
        public string Host { get; }
        public int Port { get; }

        public KafkaHostAddress(string host, int port)
        {
            this.Host = !string.IsNullOrWhiteSpace(host) ?
                host :
                throw new ArgumentException("The host value must me filled", nameof(host));

            this.Port = port;
        }

        public static KafkaHostAddress Parse(string address) // TODO
        {
            throw new NotImplementedException();
        }
    }
}

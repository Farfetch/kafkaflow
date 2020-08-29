namespace KafkaFlow.Client
{
    using System;

    public class BrokerAddress
    {
        public string Host { get; }
        public int Port { get; }

        public BrokerAddress(string host, int port)
        {
            this.Host = !string.IsNullOrWhiteSpace(host) ?
                host :
                throw new ArgumentException("The host value must me filled", nameof(host));

            this.Port = port;
        }

        public static BrokerAddress Parse(string address)
        {
            throw new NotImplementedException();
        }
    }
}

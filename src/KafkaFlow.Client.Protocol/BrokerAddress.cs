namespace KafkaFlow.Client.Protocol
{
    using System;

    public class BrokerAddress : IEquatable<BrokerAddress>
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

        public bool Equals(BrokerAddress other)
        {
            return this.Host == other.Host && this.Port == other.Port;
        }

        public override bool Equals(object? obj)
        {
            return obj is BrokerAddress other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Host, this.Port);
        }
    }
}

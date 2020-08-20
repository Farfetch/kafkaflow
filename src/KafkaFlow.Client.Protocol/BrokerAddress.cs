namespace KafkaFlow.Client.Protocol
{
    using System;

    /// <summary>
    /// Represents an address of a broker
    /// </summary>
    public class BrokerAddress : IEquatable<BrokerAddress>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerAddress"/> class.
        /// </summary>
        /// <param name="host">The address hostname</param>
        /// <param name="port">The address port</param>
        public BrokerAddress(string host, int port)
        {
            this.Host = !string.IsNullOrWhiteSpace(host)
                ? host
                : throw new ArgumentException("The host value must me filled", nameof(host));

            this.Port = port;
        }

        /// <summary>
        /// Gets the address hostname
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the address port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Determines whether the specific object is equal to the current object
        /// </summary>
        /// <param name="other">The object to be compared</param>
        /// <returns></returns>
        public bool Equals(BrokerAddress other)
        {
            return this.Host == other.Host && this.Port == other.Port;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is BrokerAddress other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Host, this.Port);
        }
    }
}

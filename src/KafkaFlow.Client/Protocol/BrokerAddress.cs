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
        /// Parses a string address
        /// </summary>
        /// <param name="address">A string value with a format 'hostname:port'</param>
        /// <returns></returns>
        public static BrokerAddress Parse(string address)
        {
            var addressParts = address.Split(':');

            if (addressParts.Length != 2)
            {
                throw new ArgumentException(
                    $"The value must be in the format 'hostname:port' and it was '{address}'",
                    nameof(address));
            }

            if (!int.TryParse(addressParts[1], out var port))
            {
                throw new ArgumentException(
                    $"The port value must be a valid integer number and it was '{addressParts[1]}'",
                    nameof(address));
            }

            return new BrokerAddress(addressParts[0], port);
        }

        /// <summary>
        /// Determines whether the specific object is equal to the current object
        /// </summary>
        /// <param name="other">The object to be compared</param>
        /// <returns></returns>
        public bool Equals(BrokerAddress other) => this.Host == other.Host && this.Port == other.Port;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BrokerAddress other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => this.Host.GetHashCode() ^ this.Port.GetHashCode();
    }
}

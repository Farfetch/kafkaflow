namespace KafkaFlow
{
    /// <summary>
    /// Represents a Kafka message
    /// </summary>
    public readonly struct Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> struct.
        /// </summary>
        /// <param name="key"><inheritdoc cref="Key"/></param>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        public Message(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the message key
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// Gets the message value
        /// </summary>
        public object Value { get; }
    }
}

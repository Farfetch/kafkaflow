namespace KafkaFlow
{
    /// <summary>Acknowledge type</summary>
    public enum Acks
    {
        /// <summary>Only waits leader's acknowledge</summary>
        Leader,

        /// <summary>Waits acknowledge from all brokers</summary>
        All,

        /// <summary>Don't wait acknowledge</summary>
        None,
    }
}

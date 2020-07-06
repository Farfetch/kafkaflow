namespace KafkaFlow
{
    /// <summary>AutoOffsetReset enum values</summary>
    public enum AutoOffsetReset
    {
        /// <summary>Only reads new messages in the topic</summary>
        Latest,
        
        /// <summary>Reads the topic from the beginning</summary>
        Earliest
    }
}

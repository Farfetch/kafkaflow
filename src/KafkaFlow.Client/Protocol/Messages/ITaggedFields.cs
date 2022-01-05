namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    ///  Used to create tagged fields
    /// </summary>
    public interface ITaggedFields
    {
        /// <summary>
        /// Gets the list of tagged fields
        /// </summary>
        public TaggedField[] TaggedFields { get; }
    }
}

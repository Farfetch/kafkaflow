namespace KafkaFlow.Admin.WebApi.Contracts
{
    /// <summary>
    /// The request to reset the offsets
    /// </summary>
    public class ResetOffsetsRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the confirmation
        /// </summary>
        public bool Confirm { get; set; }
    }
}

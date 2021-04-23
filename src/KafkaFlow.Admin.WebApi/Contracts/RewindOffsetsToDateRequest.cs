namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System;

    /// <summary>
    /// The request to rewind offsets to a point in time
    /// </summary>
    public class RewindOffsetsToDateRequest
    {
        /// <summary>
        /// Gets or sets the point in time
        /// </summary>
        public DateTime Date { get; set; }
    }
}

namespace KafkaFlow.Admin.WebApi.Contracts
{
    /// <summary>
    /// The request to change the number of workers
    /// </summary>
    public class ChangeWorkerCountRequest
    {
        /// <summary>
        /// Gets or sets the workers count
        /// </summary>
        public int WorkerCount { get; set; }
    }
}

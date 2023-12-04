namespace KafkaFlow.Admin.WebApi.Contracts;

/// <summary>
/// The request to change the number of workers
/// </summary>
public class ChangeWorkersCountRequest
{
    /// <summary>
    /// Gets or sets the workers count
    /// </summary>
    public int WorkersCount { get; set; }
}

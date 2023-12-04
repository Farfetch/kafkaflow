using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaFlow.Admin;

/// <summary>
/// Expose Consumer administration features
/// </summary>
public interface IConsumerAdmin
{
    /// <summary>
    /// Pause all consumers from a specific group
    /// </summary>
    /// <param name="groupId">The consumer group ID</param>
    /// <param name="topics">The topic names to pause</param>
    /// <returns></returns>
    Task PauseConsumerGroupAsync(string groupId, IEnumerable<string> topics);

    /// <summary>
    /// Resume all consumers from a specific group
    /// </summary>
    /// <param name="groupId">The consumer group ID</param>
    /// <param name="topics">The topic names to pause</param>
    /// <returns></returns>
    Task ResumeConsumerGroupAsync(string groupId, IEnumerable<string> topics);

    /// <summary>
    /// Pause the consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <param name="topics">The topic names to pause</param>
    /// <returns></returns>
    Task PauseConsumerAsync(string consumerName, IEnumerable<string> topics);

    /// <summary>
    /// Resume the consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <param name="topics">The topic names to resume</param>
    /// <returns></returns>
    Task ResumeConsumerAsync(string consumerName, IEnumerable<string> topics);

    /// <summary>
    /// Starts a consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <returns></returns>
    Task StartConsumerAsync(string consumerName);

    /// <summary>
    /// Stops a consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <returns></returns>
    Task StopConsumerAsync(string consumerName);

    /// <summary>
    /// Restart the consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <returns></returns>
    Task RestartConsumerAsync(string consumerName);

    /// <summary>
    /// Reset the consumer partitions offset
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <param name="topics">The topic names to reset</param>
    /// <returns></returns>
    Task ResetOffsetsAsync(string consumerName, IEnumerable<string> topics);

    /// <summary>
    /// Rewind the consumer partitions offset to a point in time
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <param name="pointInTime">The point in time DateTime value</param>
    /// <param name="topics">The topic names to rewind</param>
    /// <returns></returns>
    Task RewindOffsetsAsync(string consumerName, DateTime pointInTime, IEnumerable<string> topics);

    /// <summary>
    /// Change the number of workers running in the consumer
    /// </summary>
    /// <param name="consumerName">The consumer unique name</param>
    /// <param name="workersCount">The new number of workers</param>
    /// <returns></returns>
    Task ChangeWorkersCountAsync(string consumerName, int workersCount);
}

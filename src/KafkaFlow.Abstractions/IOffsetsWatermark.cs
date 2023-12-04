namespace KafkaFlow;

/// <summary>
/// Interface that represents the low and high watermark offsets of a topic/partition
/// </summary>
public interface IOffsetsWatermark
{
    /// <summary>
    /// Gets the high watermark offset, which is the offset of the latest message in the topic/partition available for consumption + 1
    /// </summary>
    long High { get; }

    /// <summary>
    ///  Gets the offset of the earliest message in the topic/partition
    /// </summary>
    long Low { get; }
}

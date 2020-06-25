namespace KafkaFlow
{
    public interface IOffsetsWatermark
    {
        long High { get; }

        long Low { get; }
    }
}

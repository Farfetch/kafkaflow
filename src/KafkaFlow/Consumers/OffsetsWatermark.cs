namespace KafkaFlow.Consumers
{
    using Confluent.Kafka;

    internal readonly struct OffsetsWatermark : IOffsetsWatermark
    {
        private readonly WatermarkOffsets watermark;

        public OffsetsWatermark(WatermarkOffsets watermark)
        {
            this.watermark = watermark;
        }

        public long High => this.watermark.High.Value;

        public long Low => this.watermark.Low.Value;
    }
}

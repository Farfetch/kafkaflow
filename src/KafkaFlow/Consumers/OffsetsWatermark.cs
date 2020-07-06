namespace KafkaFlow.Consumers
{
    using System;
    using Confluent.Kafka;

    internal readonly struct OffsetsWatermark : IOffsetsWatermark, IEquatable<OffsetsWatermark>
    {
        private readonly WatermarkOffsets watermark;

        public OffsetsWatermark(WatermarkOffsets watermark)
        {
            this.watermark = watermark;
        }

        public long High => this.watermark.High.Value;

        public long Low => this.watermark.Low.Value;

        public bool Equals(OffsetsWatermark other)
        {
            return Equals(this.watermark, other.watermark);
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetsWatermark other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.watermark != null ? this.watermark.GetHashCode() : 0;
        }
    }
}

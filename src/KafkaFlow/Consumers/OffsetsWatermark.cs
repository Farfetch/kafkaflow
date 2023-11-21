using System;
using Confluent.Kafka;

namespace KafkaFlow.Consumers
{
    internal readonly struct OffsetsWatermark : IOffsetsWatermark, IEquatable<OffsetsWatermark>
    {
        private readonly WatermarkOffsets _watermark;

        public OffsetsWatermark(WatermarkOffsets watermark)
        {
            _watermark = watermark;
        }

        public long High => _watermark.High.Value;

        public long Low => _watermark.Low.Value;

        public bool Equals(OffsetsWatermark other)
        {
            return Equals(_watermark, other._watermark);
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetsWatermark other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return _watermark != null ? _watermark.GetHashCode() : 0;
        }
    }
}

using System;

namespace KafkaFlow
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime MinValue => DateTime.MinValue;
    }
}

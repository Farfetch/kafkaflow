namespace KafkaFlow
{
    using System;

    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime MinValue => DateTime.MinValue;
    }
}

namespace KafkaFlow
{
    using System;

    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime MinValue => DateTime.MinValue;
    }
}

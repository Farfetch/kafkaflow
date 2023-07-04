namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Provides access to DateTime static members
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <inheritdoc cref="DateTime.Now"/>
        DateTime Now { get; }

        /// <inheritdoc cref="DateTime.UtcNow"/>
        DateTime UtcNow { get; }

        /// <inheritdoc cref="DateTime.MinValue"/>
        DateTime MinValue { get; }
    }
}

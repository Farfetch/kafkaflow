using System;

namespace KafkaFlow
{
    /// <summary>
    /// An interface used to create a log handler
    /// </summary>
    public interface ILogHandler
    {
        /// <summary>
        /// Writes a error log entry
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception thrown</param>
        /// <param name="data">Additional data related to the error log</param>
        void Error(string message, Exception ex, object data);

        /// <summary>
        /// Writes a warning log entry
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="data">Additional data related to the warning log</param>
        void Warning(string message, object data);

        /// <summary>
        /// Writes a info log entry
        /// </summary>
        /// <param name="message">Info message</param>
        /// <param name="data">Additional data related to the info log</param>
        void Info(string message, object data);

        /// <summary>
        /// Writes a verbose log entry
        /// </summary>
        /// <param name="message">Verbose message</param>
        /// <param name="data">Additional data related to the verbose log</param>
        void Verbose(string message, object data);
    }
}

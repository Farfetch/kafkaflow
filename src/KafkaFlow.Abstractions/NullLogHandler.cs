namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Default logger that does nothing
    /// </summary>
    public class NullLogHandler : ILogHandler
    {
        /// <summary>
        /// Should write a error log entry but does nothing 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception thrown</param>
        /// <param name="data">Additional data related to the error</param>
        public void Error(string message, Exception ex, object data) { }

        /// <summary>
        /// Should write a info log entry but does nothing
        /// </summary>
        /// <param name="message">Info message</param>
        /// <param name="data">Additional data related to the info</param>
        public void Info(string message, object data) { }

        /// <summary>
        /// Should write a warning log entry but does nothing 
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="data">Additional data related to the warning</param>
        public void Warning(string message, object data) { }
    }
}

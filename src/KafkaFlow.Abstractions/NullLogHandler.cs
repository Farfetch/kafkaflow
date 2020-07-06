namespace KafkaFlow
{
    using System;

    /// <summary>
    /// A log handler that does nothing
    /// </summary>
    public class NullLogHandler : ILogHandler
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        public void Error(string message, Exception ex, object data) { }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void Info(string message, object data) { }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void Warning(string message, object data) { }
    }
}

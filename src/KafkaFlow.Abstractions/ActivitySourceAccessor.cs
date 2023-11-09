namespace KafkaFlow
{
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// ActivitySource properties
    /// </summary>
    public static class ActivitySourceAccessor
    {
        internal static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private const string _activityString = "otel_activity";

        private static readonly ActivitySource _activitySource = new("KafkaFlow.OpenTelemetry", Version);

        /// <summary>
        /// Gets the name of the OpenTelemetry Activity that is used as a key
        /// in MessageContext.Items dictionary
        /// </summary>
        public static string ActivityString => _activityString;

        /// <summary>
        /// Gets the ActivitySource name that is used in KafkaFlow
        /// </summary>
        public static ActivitySource ActivitySource => _activitySource;
    }
}

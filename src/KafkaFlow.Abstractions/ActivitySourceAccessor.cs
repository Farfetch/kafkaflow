namespace KafkaFlow
{
    using System.Diagnostics;
    using System.Reflection;

    public static class ActivitySourceAccessor
    {
        public const string ActivityString = "otel_activity";
        public static readonly ActivitySource ActivitySource = new("KafkaFlow.OpenTelemetry", Version);
        internal static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}

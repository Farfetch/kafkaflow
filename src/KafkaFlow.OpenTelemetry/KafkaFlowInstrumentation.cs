using System.Reflection;

namespace KafkaFlow.OpenTelemetry
{
    /// <summary>
    /// KafkaFlow OTEL instrumentation properties
    /// </summary>
    public static class KafkaFlowInstrumentation
    {
        internal static readonly AssemblyName AssemblyName = typeof(KafkaFlowInstrumentation).Assembly.GetName();
        internal static readonly string Version = AssemblyName.Version.ToString();

        /// <summary>
        /// ActivitySource name to be used when adding
        /// KafkaFlow as source to an OTEL listener
        /// </summary>
        public static readonly string ActivitySourceName = AssemblyName.Name;
    }
}

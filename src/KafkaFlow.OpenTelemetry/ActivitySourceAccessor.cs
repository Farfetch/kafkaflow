extern alias SemanticConventions;

namespace KafkaFlow.OpenTelemetry
{
    using System.Diagnostics;
    using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

    internal static class ActivitySourceAccessor
    {
        internal const string ActivityString = "otel_activity";
        internal const string MessagingSystemId = "kafka";
        internal const string AttributeMessagingOperation = "messaging.operation";
        internal const string AttributeMessagingKafkaMessageKey = "messaging.kafka.message.key";
        internal const string AttributeMessagingKafkaMessageOffset = "messaging.kafka.message.offset";

        internal static readonly ActivitySource ActivitySource = new(KafkaFlowInstrumentation.ActivitySourceName, KafkaFlowInstrumentation.Version);

        internal static void SetGenericTags(Activity activity)
        {
            activity?.SetTag(Conventions.AttributeMessagingSystem, MessagingSystemId);
        }
    }
}

extern alias SemanticConventions;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

namespace KafkaFlow.OpenTelemetry
{
    internal static class ActivitySourceAccessor
    {
        internal const string ActivityString = "otel_activity";
        internal const string MessagingSystemId = "kafka";
        internal const string AttributeMessagingOperation = "messaging.operation";
        internal const string AttributeMessagingKafkaMessageKey = "messaging.kafka.message.key";
        internal const string AttributeMessagingKafkaMessageOffset = "messaging.kafka.message.offset";

        internal static readonly ActivitySource s_activitySource = new(KafkaFlowInstrumentation.ActivitySourceName, KafkaFlowInstrumentation.Version);

        internal static void SetGenericTags(Activity activity, IEnumerable<string> bootstrapServers)
        {
            activity?.SetTag(Conventions.AttributeMessagingSystem, MessagingSystemId);
            activity?.SetTag(Conventions.AttributePeerService, string.Join(",", bootstrapServers ?? Enumerable.Empty<string>()));
        }
    }
}
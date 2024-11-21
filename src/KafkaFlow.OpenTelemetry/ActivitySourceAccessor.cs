using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KafkaFlow.OpenTelemetry;

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
        // https://opentelemetry.io/docs/languages/net/libraries/#note-on-versioning
        // https://github.com/open-telemetry/opentelemetry-dotnet/blob/core-1.9.0/src/Shared/SemanticConventions.cs
        activity?.SetTag("message.type", MessagingSystemId);
        activity?.SetTag("peer.service", string.Join(",", bootstrapServers ?? Enumerable.Empty<string>()));
    }
}

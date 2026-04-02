using System.Diagnostics;
using System.Text;

namespace KafkaFlow.OpenTelemetry;

internal static class ActivitySourceAccessor
{
    internal const string ActivityContextItemKey = "otel_activity";
    internal const string MessagingSystemId = "kafka";

    internal static readonly ActivitySource s_activitySource = new(KafkaFlowInstrumentation.ActivitySourceName, KafkaFlowInstrumentation.Version);

    internal static void SetGenericTags(Activity activity)
    {
        // https://opentelemetry.io/docs/languages/net/libraries/#note-on-versioning
        // https://github.com/open-telemetry/opentelemetry-dotnet/blob/core-1.9.0/src/Shared/SemanticConventions.cs
        activity?.SetTag("messaging.system", MessagingSystemId);
    }

    internal static string FormatMessageKey(object key) => key switch
    {
        null => null,
        byte[] bytes => Encoding.UTF8.GetString(bytes),
        _ => key.ToString(),
    };
}

using System.Diagnostics;

namespace KafkaFlow
{
    internal class ActivityFactory : IActivityFactory
    {
        public Activity Start(string topicName, ActivityOperationType activityOperationType, ActivityKind activityKind)
        {
            var activityName = !string.IsNullOrEmpty(topicName) ? $"{topicName} {activityOperationType}" : activityOperationType.ToString().ToLower();

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
            return ActivitySourceAccessor.ActivitySource.StartActivity(activityName, activityKind);
        }
    }
}

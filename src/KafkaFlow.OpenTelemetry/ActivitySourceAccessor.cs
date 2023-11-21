extern alias SemanticConventions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

namespace KafkaFlow.OpenTelemetry
{
    internal static class ActivitySourceAccessor
    {
        internal const string ActivityString = "otel_activity";
        internal const string ExceptionEventKey = "exception";
        internal const string MessagingSystemId = "kafka";
        internal const string AttributeMessagingOperation = "messaging.operation";
        internal const string AttributeMessagingKafkaMessageKey = "messaging.kafka.message.key";
        internal const string AttributeMessagingKafkaMessageOffset = "messaging.kafka.message.offset";
        internal static readonly AssemblyName s_assemblyName = typeof(ActivitySourceAccessor).Assembly.GetName();
        internal static readonly string s_activitySourceName = s_assemblyName.Name;
        internal static readonly string s_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal static readonly ActivitySource s_activitySource = new(s_activitySourceName, s_version);

        public static void SetGenericTags(Activity activity, IEnumerable<string> bootstrapServers)
        {
            activity?.SetTag(Conventions.AttributeMessagingSystem, MessagingSystemId);
            activity?.SetTag(Conventions.AttributePeerService, string.Join(",", bootstrapServers ?? Enumerable.Empty<string>()));
        }

        public static ActivityEvent CreateExceptionEvent(Exception exception)
        {
            var activityTagCollection = new ActivityTagsCollection(
                new[]
                {
                    new KeyValuePair<string, object>(Conventions.AttributeExceptionMessage, exception.Message),
                    new KeyValuePair<string, object>(Conventions.AttributeExceptionStacktrace, exception.StackTrace),
                });

            return new ActivityEvent(ExceptionEventKey, DateTimeOffset.UtcNow, activityTagCollection);
        }
    }
}

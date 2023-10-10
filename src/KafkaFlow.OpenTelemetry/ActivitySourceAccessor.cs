extern alias SemanticConventions;

namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

    internal static class ActivitySourceAccessor
    {
        internal const string ActivityString = "otel_activity";
        internal const string ExceptionEventKey = "exception";
        internal const string MessagingSystemId = "kafka";
        internal const string AttributeMessagingOperation = "messaging.operation";
        internal const string AttributeMessagingKafkaMessageKey = "messaging.kafka.message.key";
        internal const string AttributeMessagingKafkaMessageOffset = "messaging.kafka.message.offset";
        internal static readonly AssemblyName AssemblyName = typeof(ActivitySourceAccessor).Assembly.GetName();
        internal static readonly string ActivitySourceName = AssemblyName.Name;
        internal static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version);

        public static void SetGenericTags(Activity activity)
        {
            activity?.SetTag(Conventions.AttributeMessagingSystem, MessagingSystemId);
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

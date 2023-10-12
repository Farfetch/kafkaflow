extern alias SemanticConventions;

namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

    internal static class KafkaFlowActivitySourceHelper
    {
        internal const string ActivityString = "otel_activity";
        internal const string ExceptionString = "exception";
        internal const string KafkaString = "kafka";
        internal static readonly AssemblyName AssemblyName = typeof(KafkaFlowActivitySourceHelper).Assembly.GetName();
        internal static readonly string ActivitySourceName = AssemblyName.Name;
        internal static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version);

        public static void SetGenericTags(Activity activity)
        {
            activity?.SetTag(Conventions.AttributeMessagingSystem, KafkaString);
        }

        public static ActivityEvent CreateExceptionEvent(Exception exception)
        {
            var activityTagCollection = new ActivityTagsCollection(
                new[]
                {
                    new KeyValuePair<string, object>(Conventions.AttributeExceptionMessage, exception.Message),
                    new KeyValuePair<string, object>(Conventions.AttributeExceptionStacktrace, exception.StackTrace),
                });

            return new ActivityEvent(ExceptionString, DateTimeOffset.UtcNow, activityTagCollection);
        }
    }
}

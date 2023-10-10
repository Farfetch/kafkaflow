extern alias SemanticConventions;

namespace KafkaFlow.OpenTelemetry.Trace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Conventions = SemanticConventions::OpenTelemetry.Trace.TraceSemanticConventions;

    public static class KafkaFlowActivitySourceHelper
    {
        internal static readonly IEnumerable<KeyValuePair<string, object>> CreationTags = new[]
        {
            new KeyValuePair<string, object>(Conventions.AttributePeerService, KafkaString),
            new KeyValuePair<string, object>(Conventions.AttributeMessagingSystem, KafkaString),
        };

        internal static readonly string KafkaFlowString = "KafkaFlow";
        internal static readonly string KafkaString = "kafka";

        private static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static readonly ActivitySource ActivitySource = new ActivitySource(KafkaFlowString, Version.ToString());

        public static void SetGenericTags(Activity activity)
        {
            activity?.SetTag("messaging.system", KafkaString);
            // TODO: Broker information below. Set values after
            activity?.SetTag("peer.service", KafkaString);
        }
    }
}

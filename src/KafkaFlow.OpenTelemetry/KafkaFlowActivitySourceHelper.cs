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
        public static readonly ActivitySource ActivitySource = new ActivitySource(KafkaFlowString, Version.ToString());

        internal static readonly string ActivityString = "activity";
        internal static readonly string KafkaFlowString = "KafkaFlow";
        internal static readonly string KafkaString = "kafka";

        internal static readonly IEnumerable<KeyValuePair<string, object>> CreationTags = new[]
        {
            new KeyValuePair<string, object>(Conventions.AttributePeerService, KafkaString),
            new KeyValuePair<string, object>(Conventions.AttributeMessagingSystem, KafkaString),
        };

        private static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static void SetGenericTags(Activity activity)
        {
            activity?.SetTag("messaging.system", KafkaString);
            // TODO: Broker information below. Set values after
            activity?.SetTag("peer.service", KafkaString);
        }
    }
}

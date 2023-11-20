namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using global::OpenTelemetry;
    using global::OpenTelemetry.Context.Propagation;
    using global::OpenTelemetry.Trace;

    internal static class OpenTelemetryConsumerEventsHandler
    {
        private const string ProcessString = "process";
        private const string AttributeMessagingSourceName = "messaging.source.name";
        private const string AttributeMessagingKafkaConsumerGroup = "messaging.kafka.consumer.group";
        private const string AttributeMessagingKafkaSourcePartition = "messaging.kafka.source.partition";
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public static void OnConsumeStarted(IMessageContext context)
        {
            try
            {
                var activityName = !string.IsNullOrEmpty(context?.ConsumerContext.Topic) ? $"{context?.ConsumerContext.Topic} {ProcessString}" : ProcessString;

                // Extract the PropagationContext of the upstream parent from the message headers.
                var parentContext = Propagator.Extract(new PropagationContext(default, Baggage.Current), context, ExtractTraceContextIntoBasicProperties);
                Baggage.Current = parentContext.Baggage;

                // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
                // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
                // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
                var activity = ActivitySourceAccessor.ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

                foreach (var item in Baggage.Current)
                {
                    activity?.AddBaggage(item.Key, item.Value);
                }

                context?.Items.Add(ActivitySourceAccessor.ActivityString, activity);

                ActivitySourceAccessor.SetGenericTags(activity);

                if (activity != null && activity.IsAllDataRequested)
                {
                    SetConsumerTags(context, activity);
                }
            }
            catch
            {
                // If there is any failure, do not propagate the context.
            }
        }

        public static void OnConsumeCompleted(IMessageContext context)
        {
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                activity?.Dispose();
            }
        }

        public static void OnConsumeError(IMessageContext context, Exception ex)
        {
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                activity?.Dispose();
            }
        }

        private static IEnumerable<string> ExtractTraceContextIntoBasicProperties(IMessageContext context, string key)
        {
            return new[] { context.Headers.GetString(key, Encoding.UTF8) };
        }

        private static void SetConsumerTags(IMessageContext context, Activity activity)
        {
            var messageKey = Encoding.UTF8.GetString(context.Message.Key as byte[]);

            activity.SetTag(ActivitySourceAccessor.AttributeMessagingOperation, ProcessString);
            activity.SetTag(AttributeMessagingSourceName, context.ConsumerContext.Topic);
            activity.SetTag(AttributeMessagingKafkaConsumerGroup, context.ConsumerContext.GroupId);
            activity.SetTag(ActivitySourceAccessor.AttributeMessagingKafkaMessageKey, messageKey);
            activity.SetTag(ActivitySourceAccessor.AttributeMessagingKafkaMessageOffset, context.ConsumerContext.Offset);
            activity.SetTag(AttributeMessagingKafkaSourcePartition, context.ConsumerContext.Partition);
        }
    }
}

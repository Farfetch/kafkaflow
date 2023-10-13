namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using global::OpenTelemetry;
    using global::OpenTelemetry.Context.Propagation;

    internal static class OpenTelemetryConsumerEventsHandler
    {
        private const string ProcessString = "process";
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public static Task OnConsumeStarted(IMessageContext context)
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
                var activity = KafkaFlowActivitySourceHelper.ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

                foreach (var item in Baggage.Current)
                {
                    activity?.AddBaggage(item.Key, item.Value);
                }

                context?.Items.Add(KafkaFlowActivitySourceHelper.ActivityString, activity);

                KafkaFlowActivitySourceHelper.SetGenericTags(activity);

                if (activity != null && activity.IsAllDataRequested)
                {
                    SetConsumerTags(context, activity);
                }
            }
            catch
            {
                // If there is any failure, do not propagate the context.
            }

            return Task.CompletedTask;
        }

        public static Task OnConsumeCompleted(IMessageContext context)
        {
            if (context.Items.TryGetValue(KafkaFlowActivitySourceHelper.ActivityString, out var value) && value is Activity activity)
            {
                activity?.Dispose();
            }

            return Task.CompletedTask;
        }

        public static Task OnConsumeError(IMessageContext context, Exception ex)
        {
            if (context.Items.TryGetValue(KafkaFlowActivitySourceHelper.ActivityString, out var value) && value is Activity activity)
            {
                var exceptionEvent = KafkaFlowActivitySourceHelper.CreateExceptionEvent(ex);

                activity?.AddEvent(exceptionEvent);
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<string> ExtractTraceContextIntoBasicProperties(IMessageContext context, string key)
        {
            return new[] { context.Headers.GetString(key, Encoding.UTF8) };
        }

        private static void SetConsumerTags(IMessageContext context, Activity activity)
        {
            activity.SetTag("messaging.operation", ProcessString);
            activity.SetTag("messaging.source.name", context.ConsumerContext.Topic);
            activity.SetTag("messaging.kafka.consumer.group", context.ConsumerContext.GroupId);
            activity.SetTag("messaging.kafka.message.key", context.Message.Key);
            activity.SetTag("messaging.kafka.message.offset", context.ConsumerContext.Offset);
            activity.SetTag("messaging.kafka.source.partition", context.ConsumerContext.Partition);
        }
    }
}

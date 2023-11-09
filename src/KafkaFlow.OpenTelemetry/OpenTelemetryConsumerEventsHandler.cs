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
        private const string AttributeMessagingSourceName = "messaging.source.name";
        private const string AttributeMessagingKafkaConsumerGroup = "messaging.kafka.consumer.group";
        private const string AttributeMessagingKafkaSourcePartition = "messaging.kafka.source.partition";
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public static Task OnConsumeStarted(IMessageContext context)
        {
            try
            {
                // Extract the PropagationContext of the upstream parent from the message headers.
                var parentContext = Propagator.Extract(new PropagationContext(default, Baggage.Current), context, ExtractTraceContextIntoBasicProperties);
                Baggage.Current = parentContext.Baggage;

                var activity = context.Items[ActivitySourceAccessor.ActivityString] as Activity;

                if (parentContext.ActivityContext.IsValid())
                {
                    activity.SetParentId(parentContext.ActivityContext.TraceId, parentContext.ActivityContext.SpanId, parentContext.ActivityContext.TraceFlags);
                }

                foreach (var item in Baggage.Current)
                {
                    activity?.AddBaggage(item.Key, item.Value);
                }

                ActivityAccessor.SetGenericTags(activity);

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
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                activity?.Dispose();
            }

            return Task.CompletedTask;
        }

        public static Task OnConsumeError(IMessageContext context, Exception ex)
        {
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                var exceptionEvent = ActivityAccessor.CreateExceptionEvent(ex);

                activity?.AddEvent(exceptionEvent);

                activity?.Dispose();
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<string> ExtractTraceContextIntoBasicProperties(IMessageContext context, string key)
        {
            return new[] { context.Headers.GetString(key, Encoding.UTF8) };
        }

        private static void SetConsumerTags(IMessageContext context, Activity activity)
        {
            var messageKey = Encoding.UTF8.GetString(context.Message.Key as byte[]);

            activity.SetTag(ActivityAccessor.AttributeMessagingOperation, ActivityOperationType.Process.ToString().ToLower());
            activity.SetTag(AttributeMessagingSourceName, context.ConsumerContext.Topic);
            activity.SetTag(AttributeMessagingKafkaConsumerGroup, context.ConsumerContext.GroupId);
            activity.SetTag(ActivityAccessor.AttributeMessagingKafkaMessageKey, messageKey);
            activity.SetTag(ActivityAccessor.AttributeMessagingKafkaMessageOffset, context.ConsumerContext.Offset);
            activity.SetTag(AttributeMessagingKafkaSourcePartition, context.ConsumerContext.Partition);
        }
    }
}

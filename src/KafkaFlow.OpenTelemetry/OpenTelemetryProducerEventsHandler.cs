namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::OpenTelemetry;
    using global::OpenTelemetry.Context.Propagation;

    internal static class OpenTelemetryProducerEventsHandler
    {
        private const string AttributeMessagingDestinationName = "messaging.destination.name";
        private const string AttributeMessagingKafkaDestinationPartition = "messaging.kafka.destination.partition";
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public static Task OnProducerStarted(IMessageContext context)
        {
            try
            {
                var activity = context.Items[ActivitySourceAccessor.ActivityString] as Activity;

                // Depending on Sampling (and whether a listener is registered or not), the
                // activity above may not be created.
                // If it is created, then propagate its context.
                // If it is not created, the propagate the Current context, if any.
                ActivityContext contextToInject = default;

                if (activity != null)
                {
                    contextToInject = activity.Context;
                }
                else if (Activity.Current != null)
                {
                    contextToInject = Activity.Current.Context;
                }

                Baggage.Current = Baggage.Create(activity?.Baggage.ToDictionary(item => item.Key, item => item.Value));

                // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
                Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), context, InjectTraceContextIntoBasicProperties);

                ActivityAccessor.SetGenericTags(activity);

                if (activity != null && activity.IsAllDataRequested)
                {
                    SetProducerTags(context, activity);
                }
            }
            catch
            {
                // If there is any failure, do not propagate the context.
            }

            return Task.CompletedTask;
        }

        public static Task OnProducerCompleted(IMessageContext context)
        {
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                activity?.Dispose();
            }

            return Task.CompletedTask;
        }

        public static Task OnProducerError(IMessageContext context, Exception ex)
        {
            if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityString, out var value) && value is Activity activity)
            {
                var exceptionEvent = ActivityAccessor.CreateExceptionEvent(ex);

                activity?.AddEvent(exceptionEvent);

                activity?.Dispose();
            }

            return Task.CompletedTask;
        }

        private static void InjectTraceContextIntoBasicProperties(IMessageContext context, string key, string value)
        {
            if (!context.Headers.Any(x => x.Key == key))
            {
                context.Headers.SetString(key, value, Encoding.ASCII);
            }
        }

        private static void SetProducerTags(IMessageContext context, Activity activity)
        {
            activity.SetTag(ActivityAccessor.AttributeMessagingOperation, ActivityOperationType.Publish.ToString().ToLower());
            activity.SetTag(AttributeMessagingDestinationName, context?.ProducerContext.Topic);
            activity.SetTag(AttributeMessagingKafkaDestinationPartition, context?.ProducerContext.Partition);
            activity.SetTag(ActivityAccessor.AttributeMessagingKafkaMessageKey, context?.Message.Key);
            activity.SetTag(ActivityAccessor.AttributeMessagingKafkaMessageOffset, context?.ProducerContext.Offset);
        }
    }
}

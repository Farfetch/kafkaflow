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
        private const string PublishString = "publish";
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public static Task OnProducerStarted(IMessageContext context)
        {
            try
            {
                var activityName = !string.IsNullOrEmpty(context?.ProducerContext.Topic) ? $"{context?.ProducerContext.Topic} {PublishString}" : PublishString;

                // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
                // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
                // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
                var activity = KafkaFlowActivitySourceHelper.ActivitySource.StartActivity(activityName, ActivityKind.Producer);

                context?.Items.Add(KafkaFlowActivitySourceHelper.ActivityString, activity);

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

                KafkaFlowActivitySourceHelper.SetGenericTags(activity);

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
            if (context.Items.TryGetValue(KafkaFlowActivitySourceHelper.ActivityString, out var value) && value is Activity activity)
            {
                activity.Dispose();
            }

            return Task.CompletedTask;
        }

        public static Task OnProducerError(IMessageContext context, Exception ex)
        {
            if (context.Items.TryGetValue(KafkaFlowActivitySourceHelper.ActivityString, out var value) && value is Activity activity)
            {
                var exceptionEvent = KafkaFlowActivitySourceHelper.CreateExceptionEvent(ex);

                activity?.AddEvent(exceptionEvent);
            }

            return Task.CompletedTask;
        }

        private static void InjectTraceContextIntoBasicProperties(IMessageContext context, string key, string value)
        {
            try
            {
                if (!context.Headers.Any(x => x.Key == key))
                {
                    context.Headers.SetString(key, value, Encoding.ASCII);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}. Failed to inject trace context.");
            }
        }

        private static void SetProducerTags(IMessageContext context, Activity activity)
        {
            activity.SetTag("messaging.operation", PublishString);
            activity.SetTag("messaging.destination.name", context?.ProducerContext.Topic);
            activity.SetTag("messaging.kafka.destination.partition", context?.ProducerContext.Partition);
            activity.SetTag("messaging.kafka.message.key", context?.Message.Key);
            activity.SetTag("messaging.kafka.message.offset", context?.ProducerContext.Offset);
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace KafkaFlow.OpenTelemetry;

internal static class OpenTelemetryProducerEventsHandler
{
    private const string PublishString = "publish";
    private const string AttributeMessagingDestinationName = "messaging.destination.name";
    private const string AttributeMessagingKafkaDestinationPartition = "messaging.kafka.destination.partition";
    private static readonly TextMapPropagator s_propagator = Propagators.DefaultTextMapPropagator;

    public static Task OnProducerStarted(IMessageContext context, KafkaFlowInstrumentationOptions options)
    {
        try
        {
            var activityName = !string.IsNullOrEmpty(context?.ProducerContext.Topic) ? $"{context?.ProducerContext.Topic} {PublishString}" : PublishString;

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
            var activity = ActivitySourceAccessor.s_activitySource.StartActivity(activityName, ActivityKind.Producer);

            // Depending on Sampling (and whether a listener is registered or not), the
            // activity above may not be created.
            // If it is created, then propagate its context.
            // If it is not created, the propagate the Current context, if any.
            ActivityContext contextToInject = default;

            if (activity != null)
            {
                context?.Items.Add(ActivitySourceAccessor.ActivityString, activity);

                contextToInject = activity.Context;

                // Existing keys in Baggage.Current are preserved, values from Activity.Baggage are only added or overwrite if present.
                foreach (var item in activity.Baggage)
                {
                    Baggage.SetBaggage(item.Key, item.Value);
                }
            }
            else if (Activity.Current != null)
            {
                contextToInject = Activity.Current.Context;
            }

            // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
            s_propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), context, InjectTraceContextIntoBasicProperties);

            ActivitySourceAccessor.SetGenericTags(activity, context?.Brokers);

            if (activity != null && activity.IsAllDataRequested)
            {
                SetProducerTags(context, activity);
            }

            options?.EnrichProducer?.Invoke(activity, context);
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
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);

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
        activity.SetTag(ActivitySourceAccessor.AttributeMessagingOperation, PublishString);
        activity.SetTag(AttributeMessagingDestinationName, context?.ProducerContext.Topic);
        activity.SetTag(AttributeMessagingKafkaDestinationPartition, context?.ProducerContext.Partition);
        activity.SetTag(ActivitySourceAccessor.AttributeMessagingKafkaMessageKey, context?.Message.Key);
        activity.SetTag(ActivitySourceAccessor.AttributeMessagingKafkaMessageOffset, context?.ProducerContext.Offset);
    }
}
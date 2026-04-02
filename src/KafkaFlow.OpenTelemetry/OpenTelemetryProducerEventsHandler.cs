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
    private const string SendString = "send";
    private const string PublishString = "publish";
    private static readonly TextMapPropagator s_propagator = Propagators.DefaultTextMapPropagator;

    public static Task OnProducerStarted(IMessageContext context, KafkaFlowInstrumentationOptions options)
    {
        try
        {
            var activityName = !string.IsNullOrEmpty(context?.ProducerContext.Topic) ? $"{context.ProducerContext.Topic} {PublishString}" : PublishString;

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
                context?.Items.Add(ActivitySourceAccessor.ActivityContextItemKey, activity);

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

            ActivitySourceAccessor.SetGenericTags(activity);

            if (activity is { IsAllDataRequested: true })
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
        if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityContextItemKey, out var value) && value is Activity activity)
        {
            activity.Dispose();
        }

        return Task.CompletedTask;
    }

    public static Task OnProducerError(IMessageContext context, Exception ex)
    {
        if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityContextItemKey, out var value) && value is Activity activity)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.SetTag(AttributeKeys.ErrorType, ex.GetType().FullName);
            activity.AddException(ex);
            activity.Dispose();
        }

        return Task.CompletedTask;
    }

    private static void InjectTraceContextIntoBasicProperties(IMessageContext context, string key, string value)
    {
        if (context.Headers.All(x => x.Key != key))
        {
            context.Headers.SetString(key, value, Encoding.ASCII);
        }
    }

    private static void SetProducerTags(IMessageContext context, Activity activity)
    {
        activity.SetTag(AttributeKeys.OperationType, SendString);
        activity.SetTag(AttributeKeys.OperationName, PublishString);
        activity.SetTag(AttributeKeys.DestinationName, context?.ProducerContext.Topic);

        if (context?.ProducerContext.Partition.HasValue == true)
        {
            activity.SetTag(AttributeKeys.DestinationPartitionId, context.ProducerContext.Partition.Value.ToString());
        }

        if (context?.ProducerContext.Offset.HasValue == true)
        {
            activity.SetTag(AttributeKeys.KafkaOffset, context.ProducerContext.Offset);
        }

        var messageKey = ActivitySourceAccessor.FormatMessageKey(context?.Message.Key);

        if (messageKey != null)
        {
            activity.SetTag(AttributeKeys.KafkaMessageKey, messageKey);
        }

        if (context?.Message.Value == null)
        {
            activity.SetTag(AttributeKeys.KafkaMessageTombstone, true);
        }
    }
}

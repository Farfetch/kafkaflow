using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace KafkaFlow.OpenTelemetry;

internal static class OpenTelemetryConsumerEventsHandler
{
    private const string ProcessString = "process";
    private static readonly TextMapPropagator s_propagator = Propagators.DefaultTextMapPropagator;

    public static Task OnConsumeStarted(IMessageContext context, KafkaFlowInstrumentationOptions options)
    {
        try
        {
            var activityName = !string.IsNullOrEmpty(context?.ConsumerContext.Topic) ? $"{context?.ConsumerContext.Topic} {ProcessString}" : ProcessString;

            // Extract the PropagationContext of the upstream parent from the message headers.
            var parentContext = s_propagator.Extract(new PropagationContext(default, Baggage.Current), context, ExtractTraceContextIntoBasicProperties);
            Baggage.Current = parentContext.Baggage;

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
            var activity = ActivitySourceAccessor.s_activitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

            foreach (var item in Baggage.Current)
            {
                activity?.AddBaggage(item.Key, item.Value);
            }

            context?.Items.Add(ActivitySourceAccessor.ActivityContextItemKey, activity);

            ActivitySourceAccessor.SetGenericTags(activity);

            if (activity is { IsAllDataRequested: true })
            {
                SetConsumerTags(context, activity);
            }

            options?.EnrichConsumer?.Invoke(activity, context);
        }
        catch
        {
            // If there is any failure, do not propagate the context.
        }

        return Task.CompletedTask;
    }

    public static Task OnConsumeCompleted(IMessageContext context)
    {
        if (context.Items.TryGetValue(ActivitySourceAccessor.ActivityContextItemKey, out var value) && value is Activity activity)
        {
            activity.Dispose();
        }

        return Task.CompletedTask;
    }

    public static Task OnConsumeError(IMessageContext context, Exception ex)
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

    private static IEnumerable<string> ExtractTraceContextIntoBasicProperties(IMessageContext context, string key)
    {
        return new[] { context.Headers.GetString(key, Encoding.UTF8) };
    }

    private static void SetConsumerTags(IMessageContext context, Activity activity)
    {
        activity.SetTag(AttributeKeys.OperationType, ProcessString);
        activity.SetTag(AttributeKeys.OperationName, ProcessString);
        activity.SetTag(AttributeKeys.DestinationName, context.ConsumerContext.Topic);
        activity.SetTag(AttributeKeys.DestinationPartitionId, context.ConsumerContext.Partition.ToString());
        activity.SetTag(AttributeKeys.ConsumerGroupName, context.ConsumerContext.GroupId);
        activity.SetTag(AttributeKeys.ClientId, context.ConsumerContext.ConsumerName);
        activity.SetTag(AttributeKeys.KafkaOffset, context.ConsumerContext.Offset);

        var messageKey = ActivitySourceAccessor.FormatMessageKey(context.Message.Key);

        if (messageKey != null)
        {
            activity.SetTag(AttributeKeys.KafkaMessageKey, messageKey);
        }

        if (context.Message.Value == null)
        {
            activity.SetTag(AttributeKeys.KafkaMessageTombstone, true);
        }

        if (context.Message.Value is byte[] body)
        {
            activity.SetTag(AttributeKeys.MessageBodySize, body.Length);
        }
    }
}

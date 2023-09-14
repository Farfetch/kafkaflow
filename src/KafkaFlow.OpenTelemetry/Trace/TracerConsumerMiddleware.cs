namespace KafkaFlow.OpenTelemetry.Trace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using global::OpenTelemetry;
    using global::OpenTelemetry.Context.Propagation;

    internal class TracerConsumerMiddleware : IMessageMiddleware
    {
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private static readonly string ProcessString = "process";

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var activityName = !string.IsNullOrEmpty(context?.ConsumerContext.Topic) ? $"{context.ConsumerContext.Topic} {ProcessString}" : ProcessString;

            // Extract the PropagationContext of the upstream parent from the message headers.
            var parentContext = Propagator.Extract(new PropagationContext(default, Baggage.Current), context, this.ExtractTraceContextIntoBasicProperties);
            Baggage.Current = parentContext.Baggage;

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
            using var activity = KafkaFlowActivitySourceHelper.ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

            try
            {
                KafkaFlowActivitySourceHelper.SetGenericTags(activity);

                if (activity != null && activity.IsAllDataRequested)
                {
                    this.SetConsumerTags(context, activity);
                }

                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                activity?.SetTag("exception.message", ex.Message);

                throw;
            }
        }

        private IEnumerable<string> ExtractTraceContextIntoBasicProperties(IMessageContext context, string key)
        {
            try
            {
                return new[] { context.Headers.GetString(key, Encoding.UTF8) };
            }
            catch
            {
                return new string[] { };
            }
        }

        private void SetConsumerTags(IMessageContext context, Activity activity)
        {
            activity.SetTag("messaging.operation", ProcessString);
            activity.SetTag("messaging.source.name", context.ConsumerContext.Topic);
            activity.SetTag("messaging.kafka.consumer.group", context.ConsumerContext.GroupId);
            activity.SetTag("messaging.kafka.message.key", context.Message.Key);
            activity.SetTag("messaging.kafka.message.offset", context.ProducerContext.Offset);
            activity.SetTag("messaging.kafka.source.partition", context.ConsumerContext.Partition);
        }
    }
}

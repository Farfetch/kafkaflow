namespace KafkaFlow.OpenTelemetry
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::OpenTelemetry;
    using global::OpenTelemetry.Context.Propagation;
    using KafkaFlow.OpenTelemetry.Trace;
    using KafkaFlow.Producers;

    internal class OpenTelemetryProducerObserver : IProducerInstrumentationObservers
    {
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private static readonly string PublishString = "publish";

        public Task OnNotification(ProducerStartedSubject subject, IMessageContext context)
        {
            try
            {
                var activityName = !string.IsNullOrEmpty(context?.ProducerContext.Topic) ? $"{context.ProducerContext.Topic} {PublishString}" : PublishString;

                // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
                // The convention also defines a set of attributes (in .NET they are mapped as `tags`) to be populated in the activity.
                // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
                var activity = KafkaFlowActivitySourceHelper.ActivitySource.StartActivity(activityName, ActivityKind.Producer);

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

                // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
                Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), context, this.InjectTraceContextIntoBasicProperties);

                KafkaFlowActivitySourceHelper.SetGenericTags(activity);

                if (activity != null && activity.IsAllDataRequested)
                {
                    this.SetProducerTags(context, activity);
                }

                Console.WriteLine(activity.TraceId);
                Console.WriteLine(activity.SpanId);
                Console.WriteLine(activity.ParentSpanId);
            }
            catch
            {
            }

            return Task.CompletedTask;
        }

        public Task OnNotification(ProducerStoppedSubject subject, VoidObject arg)
        {
            var activity = Activity.Current;

            activity?.Stop();

            return Task.CompletedTask;
        }

        public Task OnNotification(ProducerErrorSubject subject, Exception ex)
        {
            var activity = Activity.Current;

            activity?.SetTag("exception.message", ex.Message);

            return Task.CompletedTask;
        }

        private void InjectTraceContextIntoBasicProperties(IMessageContext context, string key, string value)
        {
            try
            {
                if (!context.Headers.Any(x => x.Key == key))
                {
                    Console.WriteLine("Injecting");
                    context.Headers.Add(key, Encoding.ASCII.GetBytes(value));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}. Failed to inject trace context.");
            }
        }

        private void SetProducerTags(IMessageContext context, Activity activity)
        {
            activity.SetTag("messaging.operation", PublishString);
            activity.SetTag("messaging.destination.name", context?.ProducerContext.Topic);
            activity.SetTag("messaging.kafka.destination.partition", context.ProducerContext.Partition);
            activity.SetTag("messaging.kafka.message.key", context.Message.Key);
            activity.SetTag("messaging.kafka.message.offset", context.ProducerContext.Offset);
        }
    }
}

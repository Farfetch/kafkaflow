namespace KafkaFlow.Admin
{
    internal interface ITelemetryScheduler
    {
        void Start(string telemetryId, string topicName);

        void Stop(string telemetryId);
    }
}

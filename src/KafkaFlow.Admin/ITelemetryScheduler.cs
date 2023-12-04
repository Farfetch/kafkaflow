namespace KafkaFlow.Admin;

internal interface ITelemetryScheduler
{
    void Start(string telemetryId, string topicName, int topicPartition);

    void Stop(string telemetryId);
}

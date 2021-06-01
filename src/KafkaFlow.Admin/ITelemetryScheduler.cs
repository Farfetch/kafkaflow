namespace KafkaFlow.Admin
{
    internal interface ITelemetryScheduler
    {
        void Start(string key, string topicName);

        void Stop(string key);
    }
}

namespace KafkaFlow
{
    using System.Diagnostics;

    internal interface IActivityFactory
    {
        public Activity Start(string topicName, ActivityOperationType activityOperationType, ActivityKind activityKind);
    }
}

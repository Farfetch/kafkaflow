using System.Diagnostics;

namespace KafkaFlow
{
    internal interface IActivityFactory
    {

        public Activity Start(string topicName, ActivityOperationType activityOperationType, ActivityKind activityKind);
    }
}

namespace KafkaFlow
{
    using KafkaFlow.Observer;

    /// <summary>
    /// Represents a subject specific to worker stopped events where observers can subscribe to receive notifications.
    /// </summary>
    public class WorkerStoppedSubject : Subject<WorkerStoppedSubject>
    {
    }
}
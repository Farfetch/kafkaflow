namespace KafkaFlow
{
    using KafkaFlow.Observer;

    /// <summary>
    /// Represents a subject specific to worker stopping events where observers can subscribe to receive notifications.
    /// </summary>
    public class WorkerStoppingSubject : Subject<WorkerStoppingSubject>
    {
    }
}
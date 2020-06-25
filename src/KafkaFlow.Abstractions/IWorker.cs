namespace KafkaFlow
{
    using System;

    public interface IWorker
    {
        int Id { get; }

        void OnTaskCompleted(Action handler);
    }
}

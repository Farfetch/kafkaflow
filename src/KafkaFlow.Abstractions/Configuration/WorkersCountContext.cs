namespace KafkaFlow.Configuration
{
    public class WorkersCountContext
    {
        public WorkersCountContext(int PartitionsCount)
        {
            this.PartitionsCount = PartitionsCount;
        }

        public int PartitionsCount { get; }
    }
}

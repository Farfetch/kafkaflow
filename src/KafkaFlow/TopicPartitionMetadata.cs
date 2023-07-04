namespace KafkaFlow
{
    public class TopicPartitionMetadata
    {
        public TopicPartitionMetadata(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; }
    }
}

namespace KafkaFlow
{
    public class TopicPartitionMetadata
    {
        public TopicPartitionMetadata(int id)
        {
            this.Id = id;
        }

        public int Id { get; }
    }
}

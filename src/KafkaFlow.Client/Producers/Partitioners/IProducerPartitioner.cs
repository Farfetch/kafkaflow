namespace KafkaFlow.Client.Producers.Partitioners
{
    public interface IProducerPartitioner
    {
        int GetPartition(int partitionCount, byte[] key);
    }
}
namespace KafkaFlow.Client.Producers.Partitioners
{
    public class ByteSumPartitioner : IProducerPartitioner
    {
        public int GetPartition(int partitionCount, byte[] key)
        {
            var sum = 0;

            for (var i = 0; i < key.Length; ++i)
            {
                sum += key[i];
            }

            return sum % partitionCount;
        }
    }
}
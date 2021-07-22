namespace KafkaFlow.Client.Producers.Partitioners
{
    using System;

    public interface IProducerPartitioner
    {
        int GetPartition(int partitionCount, Memory<byte> key);
    }
}
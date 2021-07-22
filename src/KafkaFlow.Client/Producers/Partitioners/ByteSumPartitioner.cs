namespace KafkaFlow.Client.Producers.Partitioners
{
    using System;

    public class ByteSumPartitioner : IProducerPartitioner
    {
        public int GetPartition(int partitionCount, Memory<byte> key)
        {
            var sum = 0;

            var keySpan = key.Span;

            for (var i = 0; i < key.Length; ++i)
            {
                sum += keySpan[i];
            }

            return sum % partitionCount;
        }
    }
}

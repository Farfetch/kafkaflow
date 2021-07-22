namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages;

    public interface IProducer
    {
        Task<ProduceItem> ProduceAsync(
            string topic,
            Memory<byte> key,
            Memory<byte> value,
            Headers headers);
    }
}

namespace KafkaFlow
{
    using System;

    public interface IMessageSerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] data, Type type);
    }
}

namespace KafkaFlow
{
    public interface IMessageCompressor
    {
        byte[] Compress(byte[] data);

        byte[] Decompress(byte[] data);
    }
}

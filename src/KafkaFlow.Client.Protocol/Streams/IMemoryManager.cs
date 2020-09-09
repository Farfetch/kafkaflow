namespace KafkaFlow.Client.Protocol.Streams
{
    using System;

    public interface IMemoryManager
    {
        IntPtr Allocate(int size);

        void Free(IntPtr memory);
    }
}

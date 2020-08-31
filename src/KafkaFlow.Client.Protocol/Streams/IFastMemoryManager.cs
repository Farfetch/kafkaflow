namespace KafkaFlow.Client.Protocol.Streams
{
    using System;

    public interface IFastMemoryManager
    {
        IntPtr Allocate(int size);

        void Free(IntPtr memory);
    }
}

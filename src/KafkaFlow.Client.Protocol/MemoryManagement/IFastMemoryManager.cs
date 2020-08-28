namespace KafkaFlow.Client.Protocol.MemoryManagement
{
    using System;

    public interface IFastMemoryManager
    {
        IntPtr Allocate(int size);

        void Free(IntPtr memory);
    }
}

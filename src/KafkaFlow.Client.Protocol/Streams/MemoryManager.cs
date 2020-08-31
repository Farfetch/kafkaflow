namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class MemoryManager : IFastMemoryManager
    {
        private MemoryManager()
        {
        }

        public static readonly MemoryManager Instance = new MemoryManager();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr Allocate(int size) => Marshal.AllocHGlobal(size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free(IntPtr memory) => Marshal.FreeHGlobal(memory);
    }
}

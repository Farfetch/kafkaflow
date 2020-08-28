namespace KafkaFlow.Client.Protocol.MemoryManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class FastMemoryManager : IFastMemoryManager
    {
        private FastMemoryManager()
        {
        }

        public static readonly FastMemoryManager Instance = new FastMemoryManager();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr Allocate(int size) => Marshal.AllocHGlobal(size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free(IntPtr memory) => Marshal.FreeHGlobal(memory);
    }
}

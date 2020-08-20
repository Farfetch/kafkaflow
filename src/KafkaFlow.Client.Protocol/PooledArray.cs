namespace KafkaFlow.Client.Protocol
{
    using System.Buffers;

    public readonly ref struct PooledArray<T>
    {
        private readonly ArrayPool<T> pool;
        private readonly T[] array;

        public PooledArray(int minimumLength, ArrayPool<T> pool)
        {
            this.pool = pool;
            this.array = pool.Rent(minimumLength);
        }

        public PooledArray(int size) : this(size, ArrayPool<T>.Shared)
        {
        }

        public T[] Array => this.array;

        public int Length => this.array.Length;

        public static implicit operator T[](PooledArray<T> input) => input.array;

        public void Dispose()
        {
            this.pool.Return(this.array);
        }
    }
}

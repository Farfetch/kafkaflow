namespace KafkaFlow.Client.Benchmark
{
    using System;
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using KafkaFlow.Client.Protocol.Streams;

    public class MemoryStream_WriteInt32
    {
        private const int N = 1024 * 1024;
        private readonly byte[] source;

        private DynamicMemoryStream dynamic;
        private StaticMemoryStream stc;
        private MemoryStream common;

        private const int times = 10_000;

        public MemoryStream_WriteInt32()
        {
            this.source = new byte[N];
            new Random(DateTime.Now.Second).NextBytes(this.source);
        }

        [IterationSetup]
        public void Setup()
        {
            this.stc = new StaticMemoryStream(MemoryManager.Instance, this.source.Length);
            this.dynamic = new DynamicMemoryStream(MemoryManager.Instance, 1024);
            this.common = new MemoryStream(this.source.Length);
        }

        [IterationCleanup]
        public void Clean()
        {
            this.dynamic.Dispose();
            this.stc.Dispose();
        }

        [Benchmark]
        public void Static()
        {
            for (var i = 0; i < times; i++)
            {
                // this.stc.Position = 0;
                this.stc.WriteInt32(i);
            }
        }

        [Benchmark]
        public void Dynamic()
        {
            for (var i = 0; i < times; i++)
            {
                this.dynamic.WriteInt32(i);
            }
        }

        [Benchmark]
        public void Common()
        {
            for (var i = 0; i < times; i++)
            {
                this.common.WriteInt32(i);
            }
        }
    }
}

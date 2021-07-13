namespace KafkaFlow.Client.Benchmark
{
    using System;
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using KafkaFlow.Client.Protocol.Streams;

    public class MemoryStream_ReadInt32
    {
        private DynamicMemoryStream dynamic;
        private StaticMemoryStream stc;
        private MemoryStream common;
        private Random randon;

        private const int bufferSize = 1024 * 1024;

        private const int times = 10_000;

        public MemoryStream_ReadInt32()
        {
            this.randon = new Random(DateTime.Now.Second);
        }

        [IterationSetup]
        public void Setup()
        {
            this.stc = new StaticMemoryStream(MemoryManager.Instance, bufferSize);
            this.dynamic = new DynamicMemoryStream(MemoryManager.Instance);
            this.common = new MemoryStream(bufferSize);

            for (var i = 0; i < times; i++)
            {
                this.stc.WriteInt32(this.randon.Next());
                this.dynamic.WriteInt32(this.randon.Next());
                this.common.WriteInt32(this.randon.Next());
            }
        }

        [IterationCleanup]
        public void Clean()
        {
            this.dynamic.Dispose();
            this.stc.Dispose();
        }

        [Benchmark]
        public void StaticNative()
        {
            this.stc.Position = 0;
            for (var i = 0; i < times; i++)
            {
                var a = this.stc.ReadInt32Native();
            }
        }

        [Benchmark]
        public void Static()
        {
            this.stc.Position = 0;
            for (var i = 0; i < times; i++)
            {
                var a = this.stc.ReadInt32();
            }
        }

        [Benchmark]
        public void Dynamic()
        {
            this.dynamic.Position = 0;
            for (var i = 0; i < times; i++)
            {
                var a = this.dynamic.ReadInt32();
            }
        }

        [Benchmark]
        public void Common()
        {
            this.common.Position = 0;
            for (var i = 0; i < times; i++)
            {
                var a = this.common.ReadInt32();
            }
        }
    }
}

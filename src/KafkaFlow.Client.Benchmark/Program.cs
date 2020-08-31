using System;

namespace KafkaFlow.Client.Benchmark
{
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using BenchmarkDotNet.Running;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Streams;

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MemoryStream_WriteInt32>();
            BenchmarkRunner.Run<MemoryStream_ReadInt32>();
            BenchmarkRunner.Run<MemoryStream_Write_SmallData>();
            BenchmarkRunner.Run<MemoryStream_Write_1MData>();
        }
    }

    // [NativeMemoryProfiler]
    // [MemoryDiagnoser]
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

    public class MemoryStream_Write_SmallData
    {
        private const int N = 1024 * 4;
        private readonly byte[] source;

        private DynamicMemoryStream dynamic;
        private StaticMemoryStream stc;
        private MemoryStream common;

        public MemoryStream_Write_SmallData()
        {
            this.source = new byte[N];
            new Random(DateTime.Now.Second).NextBytes(this.source);
        }

        [IterationSetup]
        public void Setup()
        {
            this.stc = new StaticMemoryStream(MemoryManager.Instance, this.source.Length);
            this.dynamic = new DynamicMemoryStream(MemoryManager.Instance, 1024 * 8);
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
            this.stc.Write(this.source);
        }

        [Benchmark]
        public void Dynamic()
        {
            this.dynamic.Write(this.source);
        }

        [Benchmark]
        public void Common()
        {
            this.common.Write(this.source);
        }
    }

    public class MemoryStream_Write_1MData
    {
        private const int N = 1024 * 1024;
        private readonly byte[] source;

        private DynamicMemoryStream dynamic;
        private StaticMemoryStream stc;
        private MemoryStream common;

        public MemoryStream_Write_1MData()
        {
            this.source = new byte[N];
            new Random(DateTime.Now.Second).NextBytes(this.source);
        }

        [IterationSetup]
        public void Setup()
        {
            this.stc = new StaticMemoryStream(MemoryManager.Instance, this.source.Length);
            this.dynamic = new DynamicMemoryStream(MemoryManager.Instance, 1024 * 8);
            this.common = new MemoryStream();
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
            this.stc.Write(this.source);
        }

        [Benchmark]
        public void Dynamic()
        {
            this.dynamic.Write(this.source);
        }

        [Benchmark]
        public void Common()
        {
            this.common.Write(this.source);
        }
    }
}

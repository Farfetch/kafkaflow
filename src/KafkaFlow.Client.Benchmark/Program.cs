using System;

namespace KafkaFlow.Client.Benchmark
{
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using BenchmarkDotNet.Running;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.MemoryManagement;

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FastMemoryStreamVsMemoryStream>();
        }
    }

    // [NativeMemoryProfiler]
    // [MemoryDiagnoser]
    public class FastMemoryStreamVsMemoryStream
    {
        private const int N = 1024 * 1024;
        private readonly byte[] source;

        private FastMemoryStream fast;
        private MemoryStream common;

        private const int times = 10_000;

        public FastMemoryStreamVsMemoryStream()
        {
            this.source = new byte[N];
            new Random(DateTime.Now.Second).NextBytes(this.source);
        }

        [IterationSetup]
        public void Setup()
        {
            this.fast = new FastMemoryStream(FastMemoryManager.Instance, 1024 * 8);
            this.common = new MemoryStream();
        }

        [IterationCleanup]
        public void Clean()
        {
            this.fast.Dispose();
        }

        [Benchmark]
        public void Fast_Write_Array()
        {
            this.fast.Write(this.source);
        }

        [Benchmark]
        public void Common_Write_Array()
        {
            this.common.Write(this.source);
        }

        [Benchmark]
        public void Fast_Write_FragmentedData()
        {
            for (int i = 0; i < times; i++)
            {
                this.fast.WriteInt32(i);
            }
        }

        [Benchmark]
        public void Common_Write_FragmentedData()
        {
            for (int i = 0; i < times; i++)
            {
                this.common.WriteInt32(i);
            }
        }
    }
}

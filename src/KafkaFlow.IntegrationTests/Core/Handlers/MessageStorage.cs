namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using MessageTypes;

    internal static class MessageStorage
    {
        private const int TimeoutSec = 20;
        private static readonly ConcurrentBag<ITestMessage> TestMessages = new();
        private static readonly ConcurrentBag<LogMessages2> AvroMessages = new();
        private static readonly ConcurrentBag<TestProtoMessage> ProtoMessages = new();
        private static readonly ConcurrentBag<(long, int)> Versions = new();
        private static readonly ConcurrentBag<byte[]> ByteMessages = new();

        public static void Add(ITestMessage message)
        {
            Versions.Add((DateTime.Now.Ticks, message.Version));
            TestMessages.Add(message);
        }

        public static void Add(LogMessages2 message)
        {
            AvroMessages.Add(message);
        }

        public static void Add(TestProtoMessage message)
        {
            ProtoMessages.Add(message);
        }

        public static void Add(byte[] message)
        {
            ByteMessages.Add(message);
        }

        public static async Task AssertCountMessageAsync(ITestMessage message, int count)
        {
            var start = DateTime.Now;

            while (TestMessages.Count(x => x.Id == message.Id && x.Value == message.Value) != count)
            {
                if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
                {
                    Assert.Fail("Message not received.");
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public static async Task AssertMessageAsync(ITestMessage message)
        {
            var start = DateTime.Now;

            while (!TestMessages.Any(x => x.Id == message.Id && x.Value == message.Value))
            {
                if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
                {
                    Assert.Fail("Message (ITestMessage) not received");
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public static async Task AssertMessageAsync(LogMessages2 message)
        {
            var start = DateTime.Now;

            while (!AvroMessages.Any(x => x.Message == message.Message && x.Schema.Fullname == message.Schema.Fullname))
            {
                if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
                {
                    Assert.Fail("Message (LogMessages2) not received");
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public static async Task AssertMessageAsync(TestProtoMessage message)
        {
            var start = DateTime.Now;

            while (!ProtoMessages.Any(x => x.Id == message.Id && x.Value == message.Value && x.Version == message.Version))
            {
                if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
                {
                    Assert.Fail("Message (TestProtoMessage) not received");
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public static async Task AssertMessageAsync(byte[] message)
        {
            var start = DateTime.Now;

            while (!ByteMessages.Any(x => x.SequenceEqual(message)))
            {
                if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
                {
                    Assert.Fail("Message (byte[]) not received");
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public static List<(long ticks, int version)> GetVersions()
        {
            return Versions.ToList();
        }

        public static void Clear()
        {
            Versions.Clear();
            TestMessages.Clear();
            ByteMessages.Clear();
            ProtoMessages.Clear();
        }
    }
}

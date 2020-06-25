namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using Messages;

    public static class MessageStorage
    {
        private const int timeoutSec = 5;
        private static readonly ConcurrentBag<ITestMessage> testMessages = new ConcurrentBag<ITestMessage>();
        private static readonly ConcurrentBag<(long, int)> versions = new ConcurrentBag<(long, int)>();
        private static readonly ConcurrentBag<byte[]> byteMessages = new ConcurrentBag<byte[]>();

        public static void Add(ITestMessage message)
        {
            versions.Add((DateTime.Now.Ticks, message.Version));
            testMessages.Add(message);
        }
        
        public static void Add(byte[] message)
        {
            byteMessages.Add(message);
        }

        public static async Task AssertCountMessageAsync(ITestMessage message, int count)
        {
            var start = DateTime.Now;

            while (testMessages.Count(x => x.Id == message.Id && x.Value == message.Value) != count)
            {
                if (DateTime.Now.Subtract(start).Seconds > timeoutSec)
                {
                    Assert.Fail("Message not received.");
                    return;
                }

                await Task.Delay(100);
            }
        }

        public static async Task AssertMessageAsync(ITestMessage message)
        {
            var start = DateTime.Now;

            while (!testMessages.Any(x => x.Id == message.Id && x.Value == message.Value))
            {
                if (DateTime.Now.Subtract(start).Seconds > timeoutSec)
                {
                    Assert.Fail("Message not received");
                    return;
                }

                await Task.Delay(100);
            }
        }

        public static async Task AssertMessageAsync(byte[] message)
        {
            var start = DateTime.Now;

            while (!byteMessages.Any(x => x.SequenceEqual(message)))
            {
                if (DateTime.Now.Subtract(start).Seconds > timeoutSec)
                {
                    Assert.Fail("Message not received");
                    return;
                }

                await Task.Delay(100);
            }
        }

        public static List<(long ticks, int version)> GetVersions()
        {
            return versions.ToList();
        }

        public static void Clear()
        {
            versions.Clear();
            testMessages.Clear();
            byteMessages.Clear();
        }
    }
}

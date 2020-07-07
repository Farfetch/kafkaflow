namespace KafkaFlow.IntegrationTests.Common.Core.Handlers
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
        private static readonly ConcurrentBag<(ITestMessage Message, long ticks)> messages = new ConcurrentBag<(ITestMessage, long)>();
        private static readonly ConcurrentBag<byte[]> byteMessages = new ConcurrentBag<byte[]>();

        public static void Add(ITestMessage message)
        {
            messages.Add((message, DateTime.Now.Ticks));
        }
        
        public static void Add(byte[] message)
        {
            byteMessages.Add(message);
        }

        public static async Task AssertCountMessagesAsync(ITestMessage message, int count)
        {
            await AssertAsync((messages) => messages.Count(x => x.Id == message.Id && x.Value == message.Value) != count);
        }

        public static async Task AssertMessageEqualAsync(ITestMessage message)
        {
            await AssertAsync((messages) => !messages.Any(x => x.Id == message.Id && x.Value == message.Value));
        }

        private static async Task AssertAsync(Func<IEnumerable<ITestMessage>, bool> action)
        {
            var start = DateTime.Now;

            while (action(messages.Select(m=> m.Message)))
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

        public static IEnumerable<int> GetVersionsFromMessage(Guid messageId)
        {
            return messages.Where(m => m.Message.Id == messageId).OrderBy(m => m.ticks).Select(m => m.Message.Version);
        }

        public static void Clear()
        {
            messages.ToList().Clear();
            byteMessages.ToList().Clear();
        }
    }
}

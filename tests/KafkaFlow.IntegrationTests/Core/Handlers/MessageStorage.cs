using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaFlow.IntegrationTests.Core.Messages;
using MessageTypes;

namespace KafkaFlow.IntegrationTests.Core.Handlers;

internal static class MessageStorage
{
    private const int TimeoutSec = 8;
    private static readonly ConcurrentBag<ITestMessage> s_testMessages = new();
    private static readonly ConcurrentBag<LogMessages2> s_avroMessages = new();
    private static readonly ConcurrentBag<TestProtoMessage> s_protoMessages = new();
    private static readonly ConcurrentBag<(long, int)> s_versions = new();
    private static readonly ConcurrentBag<byte[]> s_byteMessages = new();
    private static readonly ConcurrentBag<Message> s_nullMessages = new();

    public static void Add(ITestMessage message)
    {
        s_versions.Add((DateTime.Now.Ticks, message.Version));
        s_testMessages.Add(message);
    }

    public static void Add(LogMessages2 message)
    {
        s_avroMessages.Add(message);
    }

    public static void Add(TestProtoMessage message)
    {
        s_protoMessages.Add(message);
    }

    public static void Add(byte[] message)
    {
        s_byteMessages.Add(message);
    }

    public static void AddNullMessage(Message message)
    {
        s_nullMessages.Add(message);
    }

    public static async Task AssertCountMessageAsync(ITestMessage message, int count)
    {
        var start = DateTime.Now;

        while (s_testMessages.Count(x => x.Id == message.Id && x.Value == message.Value) != count)
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

        while (!s_testMessages.Any(x => x.Id == message.Id && x.Value == message.Value))
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

        while (!s_avroMessages.Any(x => x.Message == message.Message && x.Schema.Fullname == message.Schema.Fullname))
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

        while (!s_protoMessages.Any(x => x.Id == message.Id && x.Value == message.Value && x.Version == message.Version))
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

        while (!s_byteMessages.Any(x => x.SequenceEqual(message)))
        {
            if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
            {
                Assert.Fail("Message (byte[]) not received");
                return;
            }

            await Task.Delay(100).ConfigureAwait(false);
        }
    }

    public static async Task AssertNullMessageAsync()
    {
        var start = DateTime.Now;
        while (s_nullMessages.IsEmpty)
        {
            if (DateTime.Now.Subtract(start).Seconds > TimeoutSec)
            {
                Assert.Fail("Null message not received");
                return;
            }

            await Task.Delay(100).ConfigureAwait(false);
        }
    }

    public static List<(long ticks, int version)> GetVersions()
    {
        return s_versions.ToList();
    }

    public static void Clear()
    {
        s_versions.Clear();
        s_testMessages.Clear();
        s_byteMessages.Clear();
        s_protoMessages.Clear();
    }
}

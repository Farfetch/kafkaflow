using System;
using System.Threading.Tasks;

namespace KafkaFlow.UnitTests;

public static class ExtensionHelpers
{
    public static TaskCompletionSource WithTimeout(this TaskCompletionSource taskCompletionSource, int milliseconds)
    {
        Task.Delay(milliseconds).ContinueWith(_ => taskCompletionSource.TrySetException(new TimeoutException()));
        return taskCompletionSource;
    }
}

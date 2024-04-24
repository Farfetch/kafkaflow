using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaFlow.Extensions;

internal static class TaskExtensions
{
    public static async Task WithCancellation(
        this Task task,
        CancellationToken cancellationToken,
        bool throwOperationCancelledException)
    {
        var tcs = new TaskCompletionSource<object>();

        using var ctr = cancellationToken.Register(TrySetResult);

        if (cancellationToken.IsCancellationRequested)
        {
            TrySetResult();
            return;
        }

        await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);

        void TrySetResult()
        {
            if (throwOperationCancelledException)
            {
                tcs.TrySetException(new OperationCanceledException(cancellationToken));
            }
            else
            {
                tcs.TrySetResult(null);
            }
        }
    }
}

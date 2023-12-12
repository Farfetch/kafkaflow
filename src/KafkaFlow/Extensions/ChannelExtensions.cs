using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;

namespace KafkaFlow.Extensions;

internal static class ChannelExtensions
{
    public static async IAsyncEnumerable<T> ReadAllItemsAsync<T>(
        this ChannelReader<T> reader,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (await reader.WaitToReadAsync(cancellationToken))
        {
            while (reader.TryRead(out var item))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }
}

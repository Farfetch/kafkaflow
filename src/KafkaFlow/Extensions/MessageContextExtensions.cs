namespace KafkaFlow.Extensions;

internal static class MessageContextExtensions
{
    public static void Discard(this IConsumerContext context)
    {
        context.ShouldStoreOffset = false;
        context.Complete();
    }
}

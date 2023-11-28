namespace KafkaFlow.Sample.PauseConsumerOnError;

public class MessageHandler : IMessageHandler<WelcomeMessage>
{
    public Task Handle(IMessageContext context, WelcomeMessage message)
    {
        if (string.IsNullOrEmpty(message.Text))
            throw new InvalidDataException("Missing Message Text");

        // Handle message
        // ...

        Console.WriteLine(
            "Partition: {0} / Offset: {1} / Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        return Task.CompletedTask;
    }
}
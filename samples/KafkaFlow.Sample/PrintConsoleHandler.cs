namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        public Task Handle(IMessageContext context, TestMessage message)
        {
            var watermark = context.Consumer.GetOffsetsWatermark();

            Console.WriteLine(
                "Watermark: {0} | Offset: {1} | Lag: {2} | Message: {3}",
                watermark.High,
                context.Offset,
                watermark.High - context.Offset.Value - 1,
                message.Text);

            return Task.CompletedTask;
        }
    }
}

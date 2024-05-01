using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Authentication;
using KafkaFlow.Configuration;
using KafkaFlow.Extensions;

namespace KafkaFlow.Producers;

internal class MessageProducer : IMessageProducer, IAsyncDisposable
{
    private readonly Channel<ProduceItem> _queue = Channel.CreateUnbounded<ProduceItem>();
    private readonly object _producerCreationSync = new();

    private readonly IDependencyResolverScope _producerDependencyScope;
    private readonly ILogHandler _logHandler;
    private readonly IProducerConfiguration _configuration;
    private readonly MiddlewareExecutor _middlewareExecutor;
    private readonly GlobalEvents _globalEvents;
    private readonly Task _produceTask;

    private volatile IProducer<byte[], byte[]> _producer;

    public MessageProducer(
        IDependencyResolver dependencyResolver,
        IProducerConfiguration configuration)
    {
        _producerDependencyScope = dependencyResolver.CreateScope();
        _logHandler = dependencyResolver.Resolve<ILogHandler>();
        _configuration = configuration;
        _middlewareExecutor = new MiddlewareExecutor(configuration.MiddlewaresConfigurations);
        _globalEvents = dependencyResolver.Resolve<GlobalEvents>();

        _produceTask = Task.Run(async () =>
        {
            await foreach (var item in _queue.Reader.ReadAllItemsAsync())
            {
                IMessageContext context;

                try
                {
                    context = await item.MiddlewareCompletion;
                }
                catch
                {
                    continue;
                }

                InternalProduce(item, context);
            }
        });
    }

    public string ProducerName => _configuration.Name;

    public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        int? partition = null)
    {
        return InternalProduceAsync(
            topic,
            messageKey,
            messageValue,
            headers: headers,
            partition: partition);
    }

    public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        int? partition = null)
    {
        if (string.IsNullOrWhiteSpace(_configuration.DefaultTopic))
        {
            throw new InvalidOperationException(
                $"There is no default topic defined for producer {ProducerName}");
        }

        return InternalProduceAsync(
            _configuration.DefaultTopic,
            messageKey,
            messageValue,
            headers: headers,
            partition: partition);
    }

    public void Produce(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        _ = InternalProduceAsync(
            topic,
            messageKey,
            messageValue,
            headers: headers,
            deliveryHandler: deliveryHandler,
            partition: partition);
    }

    public void Produce(
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        if (string.IsNullOrWhiteSpace(_configuration.DefaultTopic))
        {
            throw new InvalidOperationException(
                $"There is no default topic defined for producer {ProducerName}");
        }

        _ = InternalProduceAsync(
            _configuration.DefaultTopic,
            messageKey,
            messageValue,
            headers: headers,
            deliveryHandler: deliveryHandler,
            partition: partition);
    }

    public async ValueTask DisposeAsync()
    {
        _queue.Writer.Complete();

        await _produceTask;

        _producer?.Dispose();
    }

    private static Message<byte[], byte[]> CreateMessage(IMessageContext context)
    {
        var value = context.Message.Value switch
        {
            byte[] bValue => bValue,
            null => null,
            _ => throw new InvalidOperationException(
                $"The message value must be a byte array or null to be produced, it is a {context.Message.Value.GetType().FullName}." +
                "You should serialize or encode your message object using a middleware")
        };

        var key = context.Message.Key switch
        {
            string stringKey => Encoding.UTF8.GetBytes(stringKey),
            byte[] bytesKey => bytesKey,
            null => null,
            _ => throw new InvalidOperationException(
                $"The message key must be a byte array or a string to be produced, it is a {context.Message.Key.GetType().FullName}." +
                "You should serialize or encode your message object using a middleware")
        };

        return new()
        {
            Key = key,
            Value = value,
            Headers = ((MessageHeaders)context.Headers).GetKafkaHeaders(),
            Timestamp = Timestamp.Default,
        };
    }

    private async Task<DeliveryResult<byte[], byte[]>> InternalProduceAsync(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        var middlewareCompletionSource = new TaskCompletionSource<IMessageContext>();
        var produceItem = new ProduceItem(partition, deliveryHandler, middlewareCompletionSource.Task);

        _queue.Writer.TryWrite(produceItem);

        using var messageScope = _producerDependencyScope.Resolver.CreateScope();

        var startContext = CreateMessageContext(
            topic,
            messageKey,
            messageValue,
            headers,
            messageScope.Resolver);

        await _globalEvents.FireMessageProduceStartedAsync(new MessageEventContext(startContext));

        DeliveryReport<byte[], byte[]> report = null;

        try
        {
            await _middlewareExecutor.Execute(
                startContext,
                async endContext =>
                {
                    middlewareCompletionSource.TrySetResult(endContext);
                    report = await produceItem.ProduceCompletionSource.Task;
                });
        }
        catch (Exception e)
        {
            middlewareCompletionSource.TrySetException(e);
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(startContext, e));
            throw;
        }

        await _globalEvents.FireMessageProduceCompletedAsync(new MessageEventContext(startContext));

        return report;
    }

    private void OnMessageDelivered(
        DeliveryReport<byte[], byte[]> report,
        ProduceItem item,
        IMessageContext context)
    {
        item.DeliveryHandler?.Invoke(report);

        if (report.Error.IsError)
        {
            var exception = new ProduceException<byte[], byte[]>(report.Error, report);
            OnProduceError(context, exception, report);
            item.ProduceCompletionSource.SetException(exception);
        }
        else
        {
            var concreteProducerContext = (ProducerContext)context.ProducerContext;
            concreteProducerContext.Offset = report.Offset;
            concreteProducerContext.Partition = report.Partition;

            item.ProduceCompletionSource.SetResult(report);
        }
    }

    private IProducer<byte[], byte[]> EnsureProducer()
    {
        if (_producer != null)
        {
            return _producer;
        }

        lock (_producerCreationSync)
        {
            if (_producer != null)
            {
                return _producer;
            }

            var producerBuilder = new ProducerBuilder<byte[], byte[]>(_configuration.GetKafkaConfig())
                .SetErrorHandler(
                    (_, error) =>
                    {
                        if (error.IsFatal)
                        {
                            InvalidateProducer(error, null);
                        }
                        else
                        {
                            _logHandler.Warning("Kafka Producer Error", new { Error = error });
                        }
                    })
                .SetStatisticsHandler(
                    (_, statistics) =>
                    {
                        foreach (var handler in _configuration.StatisticsHandlers)
                        {
                            handler.Invoke(statistics);
                        }
                    });

            var security = _configuration.Cluster.GetSecurityInformation();

            if (security?.OAuthBearerTokenRefreshHandler != null)
            {
                var handler = security.OAuthBearerTokenRefreshHandler;

                producerBuilder.SetOAuthBearerTokenRefreshHandler((client, _) =>
                {
                    var authenticator = new OAuthBearerAuthenticator(client);
                    handler(authenticator);
                });
            }

            return _producer = _configuration.CustomFactory(
                producerBuilder.Build(),
                _producerDependencyScope.Resolver);
        }
    }

    private void InvalidateProducer(Error error, DeliveryResult<byte[], byte[]> result)
    {
        lock (_producerCreationSync)
        {
            _producer = null;
        }

        _logHandler.Error(
            "Kafka produce fatal error occurred. The producer will be recreated",
            result is null ? new KafkaException(error) : new ProduceException<byte[], byte[]>(error, result),
            new { Error = error });
    }

    private void InternalProduce(ProduceItem item, IMessageContext context)
    {
        var producer = EnsureProducer();
        var message = CreateMessage(context);

        try
        {
            if (item.Partition.HasValue)
            {
                producer.Produce(
                    new TopicPartition(
                        context.ProducerContext.Topic,
                        item.Partition.Value),
                    message,
                    report => OnMessageDelivered(report, item, context));
            }
            else
            {
                producer.Produce(
                    context.ProducerContext.Topic,
                    message,
                    report => OnMessageDelivered(report, item, context));
            }
        }
        catch (Exception e)
        {
            OnProduceError(context, e, null);
        }
    }

    private async void OnProduceError(
        IMessageContext context,
        Exception e,
        DeliveryResult<byte[], byte[]> result)
    {
        await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(context, e));

        if (e is KafkaException kafkaException && kafkaException.Error.IsFatal)
        {
            InvalidateProducer(kafkaException.Error, result);
        }
    }

    private MessageContext CreateMessageContext(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers,
        IDependencyResolver messageScopedResolver)
    {
        return new(
            new Message(messageKey, messageValue),
            headers,
            messageScopedResolver,
            null,
            new ProducerContext(topic, _producerDependencyScope.Resolver),
            _configuration.Cluster.Brokers);
    }

    private class ProduceItem
    {
        public ProduceItem(
            int? partition,
            Action<DeliveryReport<byte[], byte[]>> deliveryHandler,
            Task<IMessageContext> middlewareCompletion)
        {
            Partition = partition;
            DeliveryHandler = deliveryHandler;
            MiddlewareCompletion = middlewareCompletion;
        }

        public int? Partition { get; }

        public Action<DeliveryReport<byte[], byte[]>> DeliveryHandler { get; }

        public Task<IMessageContext> MiddlewareCompletion { get; }

        public TaskCompletionSource<DeliveryReport<byte[], byte[]>> ProduceCompletionSource { get; } = new();
    }
}

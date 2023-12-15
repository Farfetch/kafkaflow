using System;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Authentication;
using KafkaFlow.Configuration;

namespace KafkaFlow.Producers;

internal class MessageProducer : IMessageProducer, IDisposable
{
    private readonly IDependencyResolverScope _producerDependencyScope;
    private readonly ILogHandler _logHandler;
    private readonly IProducerConfiguration _configuration;
    private readonly MiddlewareExecutor _middlewareExecutor;
    private readonly GlobalEvents _globalEvents;

    private readonly object _producerCreationSync = new();

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
    }

    public string ProducerName => _configuration.Name;

    public async Task<DeliveryResult<byte[], byte[]>> ProduceAsync(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        int? partition = null)
    {
        DeliveryResult<byte[], byte[]> report = null;

        using var messageScope = _producerDependencyScope.Resolver.CreateScope();

        var messageContext = this.CreateMessageContext(
            topic,
            messageKey,
            messageValue,
            headers,
            messageScope.Resolver);

        await _globalEvents.FireMessageProduceStartedAsync(new MessageEventContext(messageContext));

        try
        {
            await _middlewareExecutor
                .Execute(
                    messageContext,
                    async context =>
                    {
                        report = await this
                            .InternalProduceAsync(context, partition)
                            .ConfigureAwait(false);
                    })
                .ConfigureAwait(false);

            await _globalEvents.FireMessageProduceCompletedAsync(new MessageEventContext(messageContext));
        }
        catch (Exception e)
        {
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(messageContext, e));
            throw;
        }

        return report;
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
                $"There is no default topic defined for producer {this.ProducerName}");
        }

        return this.ProduceAsync(
            _configuration.DefaultTopic,
            messageKey,
            messageValue,
            headers,
            partition);
    }

    public void Produce(
        string topic,
        object messageKey,
        object messageValue,
        IMessageHeaders headers = null,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null,
        int? partition = null)
    {
        var messageScope = _producerDependencyScope.Resolver.CreateScope();

        var messageContext = this.CreateMessageContext(
            topic,
            messageKey,
            messageValue,
            headers,
            messageScope.Resolver);

        _globalEvents.FireMessageProduceStartedAsync(new MessageEventContext(messageContext));

        _middlewareExecutor
            .Execute(
                messageContext,
                context =>
                {
                    var completionSource = new TaskCompletionSource<byte>();

                    this.InternalProduce(
                        context,
                        partition,
                        report =>
                        {
                            if (report.Error.IsError)
                            {
                                completionSource.SetException(new ProduceException<byte[], byte[]>(report.Error, report));
                            }
                            else
                            {
                                completionSource.SetResult(0);
                            }

                            deliveryHandler?.Invoke(report);
                        });

                    return completionSource.Task;
                })
            .ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        deliveryHandler?.Invoke(
                            new DeliveryReport<byte[], byte[]>
                            {
                                Error = new Error(ErrorCode.Local_Fail, task.Exception?.Message),
                                Status = PersistenceStatus.NotPersisted,
                                Topic = topic,
                            });
                    }

                    messageScope.Dispose();
                });

        _globalEvents.FireMessageProduceCompletedAsync(new MessageEventContext(messageContext));
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
                $"There is no default topic defined for producer {this.ProducerName}");
        }

        this.Produce(
            _configuration.DefaultTopic,
            messageKey,
            messageValue,
            headers,
            deliveryHandler,
            partition);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }

    private static void FillContextWithResultMetadata(IMessageContext context, DeliveryResult<byte[], byte[]> result)
    {
        var concreteProducerContext = (ProducerContext)context.ProducerContext;

        concreteProducerContext.Offset = result.Offset;
        concreteProducerContext.Partition = result.Partition;
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
                            this.InvalidateProducer(error, null);
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
                var authenticator = new OAuthBearerAuthenticator();

                producerBuilder.SetOAuthBearerTokenRefreshHandler((client, _) =>
                {
                    authenticator.Client = client;

                    security.OAuthBearerTokenRefreshHandler(authenticator);
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

    private async Task<DeliveryResult<byte[], byte[]>> InternalProduceAsync(IMessageContext context, int? partition)
    {
        DeliveryResult<byte[], byte[]> result = null;

        var localProducer = this.EnsureProducer();
        var message = CreateMessage(context);

        try
        {
            var produceTask = partition.HasValue ?
                localProducer.ProduceAsync(new TopicPartition(context.ProducerContext.Topic, partition.Value), message) :
                localProducer.ProduceAsync(context.ProducerContext.Topic, message);

            result = await produceTask.ConfigureAwait(false);
        }
        catch (ProduceException<byte[], byte[]> e)
        {
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(context, e));

            if (e.Error.IsFatal)
            {
                this.InvalidateProducer(e.Error, result);
            }

            throw;
        }

        FillContextWithResultMetadata(context, result);

        return result;
    }

    private void InternalProduce(
        IMessageContext context,
        int? partition,
        Action<DeliveryReport<byte[], byte[]>> deliveryHandler)
    {
        var localProducer = this.EnsureProducer();
        var message = CreateMessage(context);

        if (partition.HasValue)
        {
            localProducer.Produce(
                new TopicPartition(context.ProducerContext.Topic, partition.Value),
                message,
                Handler);

            return;
        }

        localProducer.Produce(
            context.ProducerContext.Topic,
            message,
            Handler);

        void Handler(DeliveryReport<byte[], byte[]> report)
        {
            if (report.Error.IsFatal)
            {
                this.InvalidateProducer(report.Error, report);
            }

            FillContextWithResultMetadata(context, report);

            deliveryHandler(report);
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
}

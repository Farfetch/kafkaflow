using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Authentication;
using KafkaFlow.Configuration;
using KafkaFlow.Extensions;

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

        await _globalEvents.FireMessageProduceStartedAsync(new MessageEventContext(messageContext)).ConfigureAwait(false);

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

            await _globalEvents.FireMessageProduceCompletedAsync(new MessageEventContext(messageContext)).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(messageContext, e)).ConfigureAwait(false);
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

    public async Task<IReadOnlyCollection<BatchProduceItem>> BatchProduceAsync(
        IReadOnlyCollection<BatchProduceItem> items,
        bool throwIfAnyProduceFail = true)
    {
        if (items.Count == 0)
        {
            return items;
        }

        var batchContext = new BatchProduceContext();

        // Launch all middleware chains in parallel
        var middlewareTasks = items
            .Select((item, index) => ExecuteMessageWithBatchContextAsync(batchContext, index, item))
            .ToArray();

        // Wait for all middleware chains to complete
        // All items are now registered (either success with message, or failure with DeliveryReport set)
        await Task.WhenAll(middlewareTasks).ConfigureAwait(false);

        var allEntries = batchContext.GetEntries();
        var successfulEntries = allEntries.Where(e => e.IsSuccess).ToList();

        var errors = allEntries.Where(e => !e.IsSuccess).Select(e => e.Error).ToList();

        if (errors.Count > 0 && throwIfAnyProduceFail)
        {
            var aggregateException = new AggregateException("One or more messages failed during middleware processing", errors);
            throw new BatchProduceException(items, aggregateException);
        }

        if (successfulEntries.Count > 0)
        {
            try
            {
                await this.ProduceBatchInternalAsync(successfulEntries).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (throwIfAnyProduceFail)
                {
                    throw new BatchProduceException(items, ex);
                }

                foreach (var entry in successfulEntries.Where(e => e.Item.DeliveryReport == null))
                {
                    entry.Item.DeliveryReport = new DeliveryReport<byte[], byte[]>
                    {
                        Topic = entry.Item.Topic,
                        Error = new Error(ErrorCode.Local_Fail, ex.Message),
                        Status = PersistenceStatus.NotPersisted,
                    };
                }
            }
        }

        return allEntries.Select(e => e.Item).ToList();
    }

    private async Task ExecuteMessageWithBatchContextAsync(
        BatchProduceContext batchContext,
        int batchIndex,
        BatchProduceItem item)
    {
        using var messageScope = _producerDependencyScope.Resolver.CreateScope();

        var messageContext = this.CreateMessageContext(
            item.Topic,
            item.MessageKey,
            item.MessageValue,
            item.Headers,
            messageScope.Resolver);

        await _globalEvents.FireMessageProduceStartedAsync(new MessageEventContext(messageContext)).ConfigureAwait(false);

        try
        {
            await _middlewareExecutor
                .Execute(
                    messageContext,
                    context =>
                    {
                        // Register message for batch produce instead of producing immediately
                        var message = CreateMessage(context);
                        batchContext.Register(batchIndex, item, message, context);
                        return Task.CompletedTask;
                    })
                .ConfigureAwait(false);

            await _globalEvents.FireMessageProduceCompletedAsync(new MessageEventContext(messageContext)).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            batchContext.RegisterFailure(batchIndex, item, e);
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(messageContext, e)).ConfigureAwait(false);
        }
    }

    private void FillContextWithResultMetadata(IMessageContext context, DeliveryResult<byte[], byte[]> result)
    {
        var concreteProducerContext = context.ProducerContext as ProducerContext;

        if (concreteProducerContext is null)
        {
            _logHandler.Warning("Producer context is null on FillContextWithResultMetadata", new
            {
                DeliveryResult = result,
            });

            return;
        }

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
                            try
                            {
                                handler?.Invoke(statistics);
                            }
                            catch (Exception ex)
                            {
                                _logHandler.Error("Kafka Producer StatisticsHandler Error", ex, new
                                {
                                    ProducerName = _configuration.Name,
                                });
                            }
                        }
                    })
                .SetLogHandler((_, log) => _logHandler.Log(log));

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

    private async Task<DeliveryResult<byte[], byte[]>> InternalProduceAsync(IMessageContext context, int? partition)
    {
        DeliveryResult<byte[], byte[]> result = null;

        var localProducer = this.EnsureProducer();
        var message = CreateMessage(context);

        try
        {
            var produceTask = partition.HasValue ?
                localProducer.ProduceAsync(new Confluent.Kafka.TopicPartition(context.ProducerContext.Topic, partition.Value), message) :
                localProducer.ProduceAsync(context.ProducerContext.Topic, message);

            result = await produceTask.ConfigureAwait(false);
        }
        catch (ProduceException<byte[], byte[]> e)
        {
            await _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(context, e)).ConfigureAwait(false);

            if (e.Error.IsFatal)
            {
                this.InvalidateProducer(e.Error, result);
            }

            throw;
        }

        FillContextWithResultMetadata(context, result);

        return result;
    }

    /// <summary>
    /// Produces a batch of messages that have already been processed through middlewares.
    /// Sets DeliveryReport directly on each BatchProduceItem.
    /// </summary>
    private Task ProduceBatchInternalAsync(IReadOnlyList<BatchMessageEntry> entries)
    {
        var completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var localProducer = this.EnsureProducer();
        var pendingCount = entries.Count;

        // Produce all messages in order using synchronous Produce with callbacks
        // in order to guarantee message order in Kafka
        // https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer#produceasync-vs-produce
        foreach (var entry in entries)
        {
            void DeliveryHandler(DeliveryReport<byte[], byte[]> report)
            {
                try
                {
                    if (report.Error.IsFatal)
                    {
                        this.InvalidateProducer(report.Error, report);
                    }

                    if (report.Error.IsError)
                    {
                        _globalEvents.FireMessageProduceErrorAsync(
                            new MessageErrorEventContext(
                                entry.Context,
                                new ProduceException<byte[], byte[]>(report.Error, report)));
                    }
                    else
                    {
                        FillContextWithResultMetadata(entry.Context, report);
                    }
                }
                catch (Exception ex)
                {
                    _logHandler.Error("Batch Produce Delivery Handler Error", ex, new { Report = report });
                }
                finally
                {
                    // Set the DeliveryReport directly on the item
                    entry.Item.DeliveryReport = report;

                    if (System.Threading.Interlocked.Decrement(ref pendingCount) == 0)
                    {
                        completionSource.TrySetResult(true);
                    }
                }
            }

            try
            {
                if (entry.Item.Partition.HasValue)
                {
                    localProducer.Produce(
                        new Confluent.Kafka.TopicPartition(entry.Item.Topic, entry.Item.Partition.Value),
                        entry.MessageToProduce,
                        DeliveryHandler);
                }
                else
                {
                    localProducer.Produce(
                        entry.Item.Topic,
                        entry.MessageToProduce,
                        DeliveryHandler);
                }
            }
            catch (ProduceException<byte[], byte[]> e)
            {
                _globalEvents.FireMessageProduceErrorAsync(new MessageErrorEventContext(entry.Context, e));

                if (e.Error.IsFatal)
                {
                    this.InvalidateProducer(e.Error, null);
                }

                completionSource.TrySetException(e);
                return completionSource.Task;
            }
        }

        return completionSource.Task;
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
                new Confluent.Kafka.TopicPartition(context.ProducerContext.Topic, partition.Value),
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
            try
            {
                if (report.Error.IsFatal)
                {
                    this.InvalidateProducer(report.Error, report);
                }

                FillContextWithResultMetadata(context, report);
            }
            catch (Exception ex)
            {
                _logHandler.Error("Delivery Result Handler Error", ex, new
                {
                    Report = report,
                });
            }

            deliveryHandler?.Invoke(report);
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

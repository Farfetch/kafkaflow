﻿using System;
using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.Compressor;

/// <summary>
/// Middleware to decompress the messages when consuming
/// </summary>
public class DecompressorConsumerMiddleware : IMessageMiddleware
{
    private readonly IDecompressor _decompressor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DecompressorConsumerMiddleware"/> class.
    /// </summary>
    /// <param name="decompressor">Instance of <see cref="IDecompressor"/></param>
    public DecompressorConsumerMiddleware(IDecompressor decompressor)
    {
        _decompressor = decompressor;
    }

    /// <inheritdoc />
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        if (!(context.Message.Value is byte[] rawData))
        {
            throw new InvalidOperationException(
                $"{nameof(context.Message.Value)} must be a byte array to be decompressed and it is '{context.Message.Value.GetType().FullName}'");
        }

        var data = _decompressor.Decompress(rawData);

        return next(context.SetMessage(context.Message.Key, data));
    }
}

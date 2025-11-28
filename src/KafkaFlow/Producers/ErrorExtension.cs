using Confluent.Kafka;

namespace KafkaFlow.Producers;

/// <summary>
/// No needed
/// </summary>
public static class ErrorExtension
{
    internal static Error ToError(this IError report) => new((ErrorCode)report.Code, report.Reason, report.IsFatal);

    internal static IError ToIError(this Error report) => new ErrorFlow
    {
        Code = report.Code,
        IsError = (int)report.Code != 0,
        Reason = report.Reason,
        IsFatal = report.IsFatal,
    };
}

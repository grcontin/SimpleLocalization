using SimpleLocalization.Benchmarks.Abstractions;

namespace SimpleLocalization.Benchmarks.Internal.SimpleLocalization;

internal sealed class SimpleLocalizationService : IBenchmarkLocalizer
{
    public string GetValue() => BenchmarkMessages.GreetingMessage;

    public string GetFormattedValue(object argument) => BenchmarkMessages.UserNotification.Format(argument);
}
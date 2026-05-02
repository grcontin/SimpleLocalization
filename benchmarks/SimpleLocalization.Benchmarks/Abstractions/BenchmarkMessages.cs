namespace SimpleLocalization.Benchmarks.Abstractions;

[Localizable]
internal sealed class BenchmarkMessages
{
    [Translation("en-US", "Welcome!")]
    public static readonly LocalizableString GreetingMessage = new();

    [Translation("en-US", "You have {0} messages.")]
    public static readonly LocalizableString UserNotification = new();
}
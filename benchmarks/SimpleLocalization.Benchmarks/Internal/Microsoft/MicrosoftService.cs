using Microsoft.Extensions.Localization;
using SimpleLocalization.Benchmarks.Abstractions;

namespace SimpleLocalization.Benchmarks.Internal.Microsoft;

internal sealed class MicrosoftService(IStringLocalizer<MicrosoftService> localizer) : IBenchmarkLocalizer
{
    private readonly IStringLocalizer _localizer = localizer;

    public string GetValue() => _localizer["GreetingMessage"];

    public string GetFormattedValue(object argument) => _localizer["UserNotification", argument];
}
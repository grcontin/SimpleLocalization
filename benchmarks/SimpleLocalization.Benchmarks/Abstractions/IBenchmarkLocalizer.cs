namespace SimpleLocalization.Benchmarks.Abstractions;

internal interface IBenchmarkLocalizer
{
    string GetValue();
    string GetFormattedValue(object argument);
}
namespace SimpleLocalization;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class TranslationAttribute(string culture, string message) : Attribute
{
    public string Culture { get; } = culture;
    public string Message { get; } = message;
}
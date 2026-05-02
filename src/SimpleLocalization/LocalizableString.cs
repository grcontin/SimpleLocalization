using SimpleLocalization.Internal;

namespace SimpleLocalization;

public sealed class LocalizableString
{
    private string _absoluteKey = string.Empty;

    internal void InitializeKey(string key) => _absoluteKey = key;

    private string Value => LocalizationStore.Get(_absoluteKey);

    public static implicit operator string(LocalizableString localizable) => localizable.Value;

    public string Format(object arg0) => string.Format(Value, arg0);
    public string Format(object arg0, object arg1) => string.Format(Value, arg0, arg1);
    public string Format(object arg0, object arg1, object arg2) => string.Format(Value, arg0, arg1, arg2);
    public string Format(params object[] args) => string.Format(Value, args);

    public override string ToString() => Value;
}
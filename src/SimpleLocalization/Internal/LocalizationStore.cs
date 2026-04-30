using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace SimpleLocalization.Internal;

internal static class LocalizationStore
{
    private static Dictionary<string, Dictionary<string, string>> _store = [];

    public static void Initialize(params Assembly[] assemblies)
    {
        Dictionary<string, Dictionary<string, string>> root = [];

        foreach (Assembly assembly in assemblies)
        {
            var localizableTypes = assembly.GetTypes()
                .Where(type => type.IsDefined(typeof(LocalizableAttribute), false))
                .ToList();

            foreach (Type type in localizableTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(field => field.FieldType == typeof(LocalizableString))
                    .ToList();

                foreach (FieldInfo field in fields)
                {
                    if (field.GetValue(null) is not LocalizableString localizable)
                    {
                        continue;
                    }

                    string absoluteKey = $"{type.FullName ?? type.Name}.{field.Name}";
                    
                    localizable.InitializeKey(absoluteKey);

                    var attributes = field.GetCustomAttributes<TranslationAttribute>().ToList();

                    foreach (TranslationAttribute attribute in attributes)
                    {
                        if (!root.TryGetValue(attribute.Culture, out Dictionary<string, string>? localizations))
                        {
                            localizations = [];
                            root.Add(attribute.Culture, localizations);
                        }

                        localizations[absoluteKey] = attribute.Message;
                    }
                }
            }
        }

        _store = root.ToDictionary(
            outer => outer.Key,
            inner => inner.Value.ToDictionary()
        );
    }
    
    public static string Get(string absoluteKey)
    {
        if (string.IsNullOrWhiteSpace(absoluteKey))
        {
            return string.Empty;
        }

        CultureInfo currentCulture = CultureInfo.CurrentUICulture;

        if (_store.TryGetValue(currentCulture.Name, out Dictionary<string, string>? translations) 
            && translations.TryGetValue(absoluteKey, out string? value))
        {
            return value;
        }

        CultureInfo parentCulture = currentCulture.Parent;

        if (!parentCulture.Equals(CultureInfo.InvariantCulture) 
            && _store.TryGetValue(parentCulture.Name, out Dictionary<string, string>? parentTranslations) 
            && parentTranslations.TryGetValue(absoluteKey, out string? parentValue))
        {
            return parentValue;
        }

        return absoluteKey;
    }
}
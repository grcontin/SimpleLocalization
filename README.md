# SimpleLocalization

**SimpleLocalization** is a high-performance, strictly typed, and DI-less localization library for .NET 6+. It addresses the architectural gaps left by `IStringLocalizer`, allowing you to localize code where Dependency Injection cannot reach, such as Domain Layers and Static Contexts.

---
### Installation

Install the package via NuGet:

```bash
dotnet add package grcontin.SimpleLocalization
```

## Why SimpleLocalization?

Standard .NET localization (`IStringLocalizer`) is built around Dependency Injection. This creates significant architectural problems when you need to localize messages in layers that should remain agnostic to infrastructure.

### 1. Pure Domain Exceptions
In a clean architecture, your Domain Layer should not depend on infrastructure services. 
*   **The Problem:** To localize a `DomainException`, you would normally need to pass an `IStringLocalizer` through your entire call stack or use a Service Locator (anti-pattern).
*   **The Solution:** Use `LocalizableString` directly in your exception constructors. Your domain remains pure, and your exceptions are born with the ability to be localized.
```csharp
public sealed class DomainException : Exception
{
    public DomainException(LocalizableString message) : base(message) { }
}

// Usage in Domain Service - No DI pollution
if (balance < amount) 
    throw new BusinessException(UserErrors.InsufficientFunds);

```

### 2. Validation Attributes

Attributes are metadata evaluated at compile-time and do not support constructor injection.

*   **The Problem:** Localizing [Required] or [RegularExpression] attributes usually involves magic strings and .resx names that are fragile and hard to refactor.

*   **The Solution:** Use the static fields of your localizable classes. They provide type-safety, IDE support (find all references), and are resolved at runtime based on the current thread culture.

```csharp

public sealed class UserInput
{
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    public string Username { get; set; }
}

```

## RFC 9110 Compliance & Middleware Integration

### HTTP `Accept-Language` Header

SimpleLocalization is fully compliant with modern web standards regarding content negotiation.

The library seamlessly integrates with the standard ASP.NET Core localization pipeline. It honors the `Accept-Language` header sent by clients, following the negotiation rules defined in [RFC 9110, Section 12.5.4](https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.4).

This ensures that your API remains interoperable and follows internationalization (i18n) best practices.

### Culture Fallback Mechanism

The library implements a robust hierarchical fallback system. If a translation is missing for a specific sub-culture, it automatically traverses up the culture tree:

1. **Specific Culture** — e.g. `en-US`
2. **Parent Culture** — e.g. `en`
3. **Default / Absolute Key** — if no translation is found in the hierarchy, the library returns the fully qualified name

### 3. Static Contexts & Extension Methods

Utility classes and extension methods often need to return human-readable messages but lack access to DI.

*   **The Problem:** You cannot easily use DI inside a public static string ToRelativeTime(this DateTime date).

*   **The Solution:** Access your translations directly. SimpleLocalization is global, thread-safe, and highly optimized for static access.


## Benchmarks (.NET 10)

Comparison against `Microsoft.Extensions.Localization` using physical `.resx` files.

| Method | Mean | Ratio | Allocated |
|---|---:|---:|---:|
| Simple Localization Lookup | 15.86 ns | 0.71 | 0 B |
| IStringLocalizer Lookup (Baseline) | 22.23 ns | 1.00 | 144 B |
| Simple Localization Format | 32.73 ns | 1.47 | 88 B |
| IStringLocalizer Format | 37.73 ns | 1.70 | 256 B |

Quick Start
### 1. Define your messages

Mark your class with [Localizable] and use [Translation] attributes.

*   **Warning**: The [Localizable] attribute in this library shares the same name as the one in System.ComponentModel. To avoid unexpected behavior or compilation errors, ensure you are importing the correct namespace: using SimpleLocalization;.

```csharp
[Localizable]
public static class UserErrors
{
    [Translation("en-US", "User not found.")]
    [Translation("pt-BR", "Usuário não encontrado.")]
    public static readonly LocalizableString NotFound = new();

    [Translation("en-US", "Hello {0}, you have {1} messages.")]
    public static readonly LocalizableString Notifications = new();
}
```

### 2. Initialize

Call this once at your application startup (e.g., `Program.cs`).

You can provide multiple assemblies, which is useful when your localization messages are distributed across different projects or modules.

```csharp
services.AddSimpleLocalization(
    typeof(ApplicationAssemblyMarker).Assembly,
    typeof(DomainAssemblyMarker).Assembly
);
```

### 3. Configure Middleware (ASP.NET Core)

For the library to correctly identify the user's culture, it is crucial to configure the native .NET localization middleware. SimpleLocalization relies on the CultureInfo.CurrentUICulture set by this pipeline.

```csharp
RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US", "pt-BR")
    .AddSupportedUICultures("en-US", "pt-BR");

app.UseRequestLocalization(localizationOptions);
```

### 4. Use it anywhere

```csharp
// Implicit conversion to string
string message = UserErrors.NotFound;

// High-performance formatting (Zero-boxing for common types)
string formatted = UserErrors.Notifications.Format("Gabriel", 5);
```

using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SimpleLocalization.Internal;

namespace SimpleLocalization.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class LocalizationBenchmark
{
    private IStringLocalizer _microsoftLocalizer = null!;

    [GlobalSetup]
    public void Setup()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");

        LocalizationStore.Initialize(typeof(LocalizationBenchmark).Assembly);

        ServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddLocalization();
        
        ServiceProvider provider = services.BuildServiceProvider();
        IStringLocalizerFactory factory = provider.GetRequiredService<IStringLocalizerFactory>();
        
        _microsoftLocalizer = factory.Create(typeof(LocalizationBenchmark));
    }
    
    [Benchmark(Description = "Simple Localization Lookup")]
    public string SimpleLocalization_Lookup()
    {
        return BenchmarkMessages.Welcome;
    }
    
    [Benchmark(Description = "Simple Localization string Format")]
    public string SimpleLocalization_Format()
    {
        return BenchmarkMessages.UserNotifications.Format("Gabriel", 10);
    }

    [Benchmark(Baseline = true, Description = "IStringLocalizer Lookup")]
    public string Microsoft_Lookup()
    {
        LocalizedString result = _microsoftLocalizer["Welcome"];
        return result.Value;
    }
    
    [Benchmark(Description = "IStringLocalizer string Format")]
    public string Microsoft_Format()
    {
        LocalizedString template = _microsoftLocalizer["UserNotifications"];
        return string.Format(template.Value, "Gabriel", 10);
    }
}

[Localizable]
internal sealed class BenchmarkMessages
{
    [Translation("en-US", "Welcome!")]
    public static readonly LocalizableString Welcome = new();

    [Translation("en-US", "Hello {0}, you have {1} messages.")]
    public static readonly LocalizableString UserNotifications = new();
}
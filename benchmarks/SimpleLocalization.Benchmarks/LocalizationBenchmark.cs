using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.DependencyInjection;
using SimpleLocalization.Benchmarks.Abstractions;
using SimpleLocalization.Benchmarks.Internal.Microsoft;
using SimpleLocalization.Benchmarks.Internal.SimpleLocalization;
using SimpleLocalization.Internal;

namespace SimpleLocalization.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class LocalizationBenchmark
{
    private IBenchmarkLocalizer _microsoftService = null!;
    private IBenchmarkLocalizer _simpleService = null!;

    [GlobalSetup]
    public void Setup()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");

        LocalizationStore.Initialize(typeof(LocalizationBenchmark).Assembly);

        ServiceCollection services = new ServiceCollection();
        
        services.AddLogging();
        services.AddLocalization();
        
        services.AddSingleton<MicrosoftService>();
        services.AddSingleton<SimpleLocalizationService>();
        
        ServiceProvider provider = services.BuildServiceProvider();
        
        _microsoftService = provider.GetRequiredService<MicrosoftService>();
        _simpleService = provider.GetRequiredService<SimpleLocalizationService>();
    }
    
    [Benchmark(Description = "Simple Localization Lookup")]
    public string SimpleLocalization_Lookup()
    {
        return _simpleService.GetValue();
    }
    
    [Benchmark(Description = "Simple Localization string Format")]
    public string SimpleLocalization_Format()
    {
        return _simpleService.GetFormattedValue(10);
    }

    [Benchmark(Baseline = true, Description = "IStringLocalizer Lookup")]
    public string Microsoft_Lookup()
    {
        return _microsoftService.GetValue();
    }
    
    [Benchmark(Description = "IStringLocalizer string Format")]
    public string Microsoft_Format()
    {
        return _microsoftService.GetFormattedValue(10);
    }
}
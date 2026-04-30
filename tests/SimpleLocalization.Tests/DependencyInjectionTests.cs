using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SimpleLocalization.Extensions;

namespace SimpleLocalization.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddDomainLocalization_ShouldRegisterLocalizationServicesAndInitializeStore()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
    
        // Act
        services.AddSimpleLocalization(typeof(LocalizationStoreTests).Assembly);
        
        ServiceProvider provider = services.BuildServiceProvider();
        
        object? options = provider.GetService<IOptions<LocalizationOptions>>();
    
        // Assert
        options.Should().NotBeNull();
        TestMessages.Welcome.ToString().Should().NotBeNullOrEmpty();
    }
}
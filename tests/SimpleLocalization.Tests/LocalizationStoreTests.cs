using System.Globalization;
using System.Reflection;
using FluentAssertions;
using SimpleLocalization.Internal;

namespace SimpleLocalization.Tests;

public sealed class LocalizationStoreTests
{
    [Fact]
    public void Initialize_ShouldMapFieldsCorrectly_WhenClassIsMarkedAsLocalizable()
    {
        // Arrange
        Assembly assembly = typeof(LocalizationStoreTests).Assembly;
        
        // Act
        LocalizationStore.Initialize(assembly);

        // Assert
        TestMessages.GreetingMessage.ToString().Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [InlineData("en-US", "Welcome!")]
    [InlineData("pt-BR", "Bem-vindo!")]
    public void Get_ShouldReturnCorrectTranslation_BasedOnCulture(string cultureCode, string expected)
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);
        CultureInfo.CurrentUICulture = new CultureInfo(cultureCode);

        // Act
        string result = TestMessages.GreetingMessage;

        // Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("en-US", "Gabriel", 5, "Hello Gabriel, you have 5 messages.")]
    [InlineData("pt-BR", "Gabriel", 5, "Olá Gabriel, você tem 5 mensagens.")]
    public void Format_ShouldReturnInterpolatedString_WhenMultipleArgumentsAreProvided(
        string cultureCode, 
        string name, 
        int count, 
        string expected)
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);
        CultureInfo.CurrentUICulture = new CultureInfo(cultureCode);

        // Act
        string result = TestMessages.UserNotifications.Format(name, count);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Get_ShouldBeThreadSafe_WhenMultipleCulturesAreRequestedConcurrently()
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);

        // Act
        Task taskOne = Task.Run(() =>
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            string result = TestMessages.GreetingMessage;
            
            result.Should().Be("Bem-vindo!");
        });

        Task taskTwo = Task.Run(() =>
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            string result = TestMessages.GreetingMessage;
            
            result.Should().Be("Welcome!");
        });

        // Act & Assert
        await Task.WhenAll(taskOne, taskTwo);
    }
    
    [Fact]
    public void Get_ShouldReturnAbsoluteKey_WhenCultureIsNotRegistered()
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);
        CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
    
        // Act
        string result = TestMessages.GreetingMessage;
    
        // Assert
        result.Should().Be("SimpleLocalization.Tests.TestMessages.GreetingMessage");
    }
    
    [Fact]
    public void Initialize_ShouldNotThrow_WhenCalledMultipleTimes()
    {
        // Arrange
        Assembly assembly = typeof(LocalizationStoreTests).Assembly;

        // Act
        Action action = () =>
        {
            LocalizationStore.Initialize(assembly);
            LocalizationStore.Initialize(assembly);
        };

        // Assert
        action.Should().NotThrow();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Get_ShouldReturnEmptyString_WhenKeyIsNullOrWhiteSpace(string? invalidKey)
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);

        // Act
        string result = LocalizationStore.Get(invalidKey!);

        // Assert
        result.Should().Be(string.Empty);
    }
    
    [Fact]
    public void Initialize_ShouldMergeTranslations_FromMultipleAssemblies()
    {
        // Arrange
        Assembly currentAssembly = typeof(LocalizationStoreTests).Assembly;
        Assembly externalAssembly = typeof(Enumerable).Assembly;

        // Act
        Action action = () => LocalizationStore.Initialize(currentAssembly, externalAssembly);

        // Assert
        action.Should().NotThrow();
        TestMessages.GreetingMessage.ToString().Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public void Get_ShouldReturnParentCultureTranslation_WhenSpecificCultureIsMissing()
    {
        // Arrange
        LocalizationStore.Initialize(typeof(LocalizationStoreTests).Assembly);
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");

        // Act
        string result = TestMessages.CultureFallback;

        // Assert
        result.Should().Be("Testing Culture Fallback");
    }
}
namespace SimpleLocalization.Tests;

[Localizable]
internal static class TestMessages
{
    [Translation("en-US", "Welcome!")]
    [Translation("pt-BR", "Bem-vindo!")]
    public static readonly LocalizableString Welcome = new();
    
    [Translation("en-US", "Hello {0}, you have {1} messages.")]
    [Translation("pt-BR", "Olá {0}, você tem {1} mensagens.")]
    public static readonly LocalizableString UserNotifications = new();
    
    [Translation("en", "Testing Culture Fallback")]
    [Translation("pt", "Testando Fallback de Culture")]
    public static readonly LocalizableString CultureFallback = new();
    
}
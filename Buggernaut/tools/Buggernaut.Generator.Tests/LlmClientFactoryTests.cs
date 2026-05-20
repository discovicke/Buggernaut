using Buggernaut.Generator.Service.LLM_Clients;
using Microsoft.Extensions.Configuration;

namespace Buggernaut.Generator.Tests;

public class LlmClientFactoryTests
{
    [Theory]
    [InlineData("gemini", "Gemini")]
    [InlineData("OPENAI", "OpenAI")]
    [InlineData("mIsTrAl", "Mistral")]
    [InlineData("anthropic", "Anthropic")]
    [InlineData("ollama", "Ollama")]
    [InlineData("openrouter", "OpenRouter")]
    public void ResolveProvider_ReturnsCanonicalProviderName(string provider, string expectedProvider)
    {
        var config = BuildConfig(provider);

        Assert.Equal(expectedProvider, LlmClientFactory.ResolveProvider(config));
    }

    [Theory]
    [InlineData("gemini", typeof(GeminiClient))]
    [InlineData("OPENAI", typeof(OpenAiCompatibleClient))]
    [InlineData("mIsTrAl", typeof(OpenAiCompatibleClient))]
    [InlineData("anthropic", typeof(AnthropicClient))]
    [InlineData("ollama", typeof(OpenAiCompatibleClient))]
    [InlineData("openrouter", typeof(OpenAiCompatibleClient))]
    public void Create_ProviderNameIsCaseInsensitive(string provider, Type expectedClientType)
    {
        var config = BuildConfig(provider);

        var client = LlmClientFactory.Create(config);

        Assert.Equal(expectedClientType, client.GetType());
    }

    private static IConfiguration BuildConfig(string provider)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LLM:Provider"] = provider,
                ["LLM:Gemini:ApiKey"] = "test-key",
                ["LLM:OpenAI:ApiKey"] = "test-key",
                ["LLM:Mistral:ApiKey"] = "test-key",
                ["LLM:Anthropic:ApiKey"] = "test-key",
                ["LLM:OpenRouter:ApiKey"] = "test-key"
            })
            .Build();
    }
}

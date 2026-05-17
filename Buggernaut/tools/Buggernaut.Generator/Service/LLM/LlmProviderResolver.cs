namespace Buggernaut.Generator;

internal static class LlmProviderResolver
{
    public static string Resolve(string? provider)
    {
        return (provider ?? "Gemini").Trim().ToLowerInvariant() switch
        {
            "gemini" => "Gemini",
            "openai" => "OpenAI",
            "anthropic" => "Anthropic",
            "mistral" => "Mistral",
            "ollama" => "Ollama",
            "openrouter" => "OpenRouter",
            _ => provider?.Trim() ?? "Gemini",
        };
    }

    public static bool IsKnown(string canonical) =>
        canonical is "Gemini" or "OpenAI" or "Anthropic" or "Mistral" or "Ollama" or "OpenRouter";
}

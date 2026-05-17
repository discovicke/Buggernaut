using Buggernaut.Generator;
using Buggernaut.Generator.Service.LLM_Clients;
using Microsoft.Extensions.Configuration;

public static class LlmClientFactory
{
    public static ILlmClient Create(IConfiguration config)
    {
        var provider = LlmProviderResolver.Resolve(config["LLM:Provider"]);

        return provider switch
        {
            "Gemini" => new GeminiClient(
                config["LLM:Gemini:ApiKey"]!,
                config["LLM:Gemini:Model"] ?? "gemini-2.5-flash"),

            "OpenAI" => new OpenAiCompatibleClient(
                config["LLM:OpenAI:BaseUrl"] ?? "https://api.openai.com/v1",
                config["LLM:OpenAI:ApiKey"]!,
                config["LLM:OpenAI:Model"] ?? "gpt-4o-mini"),

            "Mistral" => new OpenAiCompatibleClient(
                config["LLM:Mistral:BaseUrl"] ?? "https://api.mistral.ai/v1",
                config["LLM:Mistral:ApiKey"]!,
                config["LLM:Mistral:Model"] ?? "mistral-small"),

            "Anthropic" => new AnthropicClient(
                config["LLM:Anthropic:ApiKey"]!,
                config["LLM:Anthropic:Model"] ?? "claude-3-5-haiku-latest"),

            "Ollama" => new OpenAiCompatibleClient(
                config["LLM:Ollama:BaseUrl"] ?? "http://localhost:11434/v1",
                "ollama",
                config["LLM:Ollama:Model"] ?? "llama3"),
            
            "OpenRouter" => new OpenAiCompatibleClient(
                config["LLM:OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1",
                config["LLM:OpenRouter:ApiKey"]!,
                config["LLM:OpenRouter:Model"] ?? "moonshotai/kimi-k2.6"),

            _ => throw new Exception($"Okänd provider: {provider}")
        };
    }
}
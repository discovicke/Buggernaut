namespace Buggernaut.Generator;

public interface ILlmClient
{
    Task<string> GenerateAsync(string prompt);
}
namespace Buggernaut.Generator.Service.LLM_Clients;

public class OpenAiCompatibleClient : ILlmClient
{
        public OpenAiCompatibleClient(string apiKey)
        {
            // Initialize the client with the API key
        }

        public Task<string> GenerateAsync(string prompt)
        {
            throw new NotImplementedException();
        }
}
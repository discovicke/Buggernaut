using Microsoft.Extensions.Configuration;

class Program
{
    static int Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var apiKey = config["Gemini:ApiKey"];
        return 0;
    }
}
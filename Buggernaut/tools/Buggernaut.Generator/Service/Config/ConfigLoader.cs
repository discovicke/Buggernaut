using Microsoft.Extensions.Configuration;

namespace Buggernaut.Generator;

internal static class ConfigLoader
{
    public static IConfiguration Load()
    {
        var solutionRoot = SolutionRootFinder.Find();
        var appSettingsPath = Path.Combine(solutionRoot, "tools", "Buggernaut.Generator", "appsettings.json");

        return new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath, optional: true)
            .AddUserSecrets(typeof(ConfigLoader).Assembly)
            .Build();
    }
}


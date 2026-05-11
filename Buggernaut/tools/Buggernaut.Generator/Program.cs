using Buggernaut.Generator;

class Program
{
    static async Task Main(string[] args)
    {
        Printer.H1("Buggernaut");

        CliArgumentParser.GenerateOptions options;
        try
        {
            options = CliArgumentParser.Parse(args);
        }
        catch (CliArgumentException ex)
        {
            if (ex.IsError)
                Printer.Error(ex.Message);
            return;
        }

        switch (options.Command)
        {
            case Command.Hint:
                MetaReader.ShowHint(options.TargetClassName);
                return;
            case Command.Explain:
                MetaReader.ShowExplanation(options.TargetClassName);
                return;
        }

        var config = ConfigLoader.Load();
        await ChallengeOrchestrator.RunAsync(options, config);
    }
}
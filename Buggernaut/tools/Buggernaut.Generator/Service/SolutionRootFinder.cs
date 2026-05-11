namespace Buggernaut.Generator;

internal static class SolutionRootFinder
{
    public static string Find()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new Exception("Kunde inte hitta solution-rooten. Finns det en .sln-fil i Buggernaut/?");
    }
}
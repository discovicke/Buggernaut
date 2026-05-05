using System.Text.Json.Serialization;

namespace Buggernaut.Generator;

public class Challenge
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("buggyCode")]
    public string BuggyCode { get; set; } = "";

    [JsonPropertyName("hint")]
    public string Hint { get; set; } = "";

    [JsonPropertyName("solutionCode")]
    public string SolutionCode { get; set; } = "";

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = "";
    
    [JsonPropertyName("testCode")]
    public string TestCode { get; set; } = "";
}
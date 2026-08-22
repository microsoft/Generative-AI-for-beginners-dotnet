using System.ComponentModel.DataAnnotations;

namespace AzFunction_BasicChat_01;

public sealed class FoundryOptions
{
    public const string SectionName = "Foundry";

    [Required]
    [Url]
    public string Endpoint { get; init; } = string.Empty;

    [Required]
    public string DeploymentName { get; init; } = string.Empty;
}

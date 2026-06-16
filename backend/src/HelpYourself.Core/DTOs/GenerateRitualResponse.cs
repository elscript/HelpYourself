using HelpYourself.Core.Models;

namespace HelpYourself.Core.DTOs;

public sealed class GenerateRitualResponse
{
    public Guid RitualId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string KeyMetaphor { get; init; } = string.Empty;
    public List<RitualPhase> Phases { get; init; } = [];
    public bool IsStatic { get; init; }

    public static GenerateRitualResponse FromRitual(Ritual ritual) => new()
    {
        RitualId = ritual.Id,
        Title = ritual.Title,
        KeyMetaphor = ritual.KeyMetaphor,
        Phases = ritual.Phases,
        IsStatic = ritual.IsStatic
    };
}

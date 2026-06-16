using HelpYourself.Core.Enums;

namespace HelpYourself.Core.Models;

public sealed class Ritual
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Archetype Archetype { get; init; }
    public string Title { get; init; } = string.Empty;
    public string KeyMetaphor { get; init; } = string.Empty;
    public List<RitualPhase> Phases { get; init; } = [];
    public bool IsStatic { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

namespace HelpYourself.Core.Models;

public sealed class RitualPhase
{
    public string Name { get; init; } = string.Empty;
    public int DurationSec { get; init; }
    public List<RitualInstruction> Instructions { get; init; } = [];
}

using HelpYourself.Core.Enums;

namespace HelpYourself.Core.Models;

public sealed class RitualInstruction
{
    public InstructionType Type { get; init; }
    public string Value { get; init; } = string.Empty;

    // For breath instructions: [inhale_sec, exhale_sec] or [inhale, hold, exhale]
    public int[]? Pattern { get; init; }
    public string? Description { get; init; }
}

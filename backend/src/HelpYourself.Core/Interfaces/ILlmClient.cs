using HelpYourself.Core.Enums;
using HelpYourself.Core.Models;

namespace HelpYourself.Core.Interfaces;

public interface ILlmClient
{
    Task<Ritual?> GenerateRitualAsync(Archetype archetype, string? context, CancellationToken ct = default);
}

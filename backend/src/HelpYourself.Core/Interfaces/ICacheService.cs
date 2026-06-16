using HelpYourself.Core.Enums;
using HelpYourself.Core.Models;

namespace HelpYourself.Core.Interfaces;

public interface ICacheService
{
    Task<Ritual?> GetCachedRitualAsync(Archetype archetype, CancellationToken ct = default);
    Task SetCachedRitualAsync(Archetype archetype, Ritual ritual, CancellationToken ct = default);
}

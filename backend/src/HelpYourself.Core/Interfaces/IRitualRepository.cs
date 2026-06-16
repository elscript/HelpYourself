using HelpYourself.Core.Enums;
using HelpYourself.Core.Models;

namespace HelpYourself.Core.Interfaces;

public interface IRitualRepository
{
    Task SaveAsync(Ritual ritual, CancellationToken ct = default);
    Task<Ritual?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveFeedbackAsync(RitualFeedback feedback, CancellationToken ct = default);
}

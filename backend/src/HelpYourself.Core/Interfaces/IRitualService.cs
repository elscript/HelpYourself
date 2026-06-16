using HelpYourself.Core.DTOs;
using HelpYourself.Core.Enums;

namespace HelpYourself.Core.Interfaces;

public interface IRitualService
{
    Task<GenerateRitualResponse> GenerateAsync(GenerateRitualRequest request, CancellationToken ct = default);
    Task RecordFeedbackAsync(Guid ritualId, FeedbackRequest feedback, CancellationToken ct = default);
    Task<GenerateRitualResponse> GetInstantRitualAsync(Archetype archetype, CancellationToken ct = default);
}

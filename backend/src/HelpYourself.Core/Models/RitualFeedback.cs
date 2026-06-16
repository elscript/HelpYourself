using HelpYourself.Core.Enums;

namespace HelpYourself.Core.Models;

public sealed class RitualFeedback
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid RitualId { get; init; }
    public FeedbackRating Rating { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

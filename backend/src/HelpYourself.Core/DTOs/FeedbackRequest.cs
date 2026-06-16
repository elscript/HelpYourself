using System.ComponentModel.DataAnnotations;
using HelpYourself.Core.Enums;

namespace HelpYourself.Core.DTOs;

public sealed class FeedbackRequest
{
    [Required]
    public FeedbackRating Rating { get; init; }
}

using System.ComponentModel.DataAnnotations;
using HelpYourself.Core.Enums;

namespace HelpYourself.Core.DTOs;

public sealed class GenerateRitualRequest
{
    [Required]
    public Archetype Archetype { get; init; }

    [MaxLength(500)]
    public string? Context { get; init; }
}

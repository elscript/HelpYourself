using System.Text.Json.Serialization;

namespace HelpYourself.Infrastructure.LLM;

// LLM raw JSON contract — kept separate from domain models
internal sealed class LlmRitualDto
{
    [JsonPropertyName("ritualId")]
    public string RitualId { get; init; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("keyMetaphor")]
    public string KeyMetaphor { get; init; } = string.Empty;

    [JsonPropertyName("phases")]
    public List<LlmPhaseDto> Phases { get; init; } = [];
}

internal sealed class LlmPhaseDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("durationSec")]
    public int DurationSec { get; init; }

    [JsonPropertyName("instructions")]
    public List<LlmInstructionDto> Instructions { get; init; } = [];
}

internal sealed class LlmInstructionDto
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("pattern")]
    public int[]? Pattern { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

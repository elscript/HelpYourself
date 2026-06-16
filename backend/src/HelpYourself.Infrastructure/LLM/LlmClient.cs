using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HelpYourself.Core.Enums;
using HelpYourself.Core.Interfaces;
using HelpYourself.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelpYourself.Infrastructure.LLM;

public sealed class LlmClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly LlmOptions _options;
    private readonly ILogger<LlmClient> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public LlmClient(HttpClient http, IOptions<LlmOptions> options, ILogger<LlmClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Ritual?> GenerateRitualAsync(Archetype archetype, string? context, CancellationToken ct = default)
    {
        var payload = BuildPayload(archetype, context);

        for (var attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _http.PostAsync(_options.ChatEndpoint, content, ct);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(ct);
                var ritual = ParseRitualFromResponse(responseBody, archetype);
                if (ritual is not null) return ritual;

                _logger.LogWarning("LLM returned invalid JSON on attempt {Attempt}, retrying...", attempt + 1);
                payload = BuildRepairPayload(responseBody);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "LLM request failed on attempt {Attempt}", attempt + 1);
                return null;
            }
        }

        return null;
    }

    private object BuildPayload(Archetype archetype, string? context) => new
    {
        model = _options.Model,
        messages = new[]
        {
            new { role = "system", content = LlmPromptBuilder.GetSystemPrompt() },
            new { role = "user", content = LlmPromptBuilder.BuildUserPrompt(archetype, context) }
        },
        temperature = 0.85,
        max_tokens = 1200
    };

    private static object BuildRepairPayload(string brokenResponse) => new
    {
        messages = new[]
        {
            new { role = "assistant", content = brokenResponse },
            new { role = "user", content = "Исправь JSON в своём предыдущем ответе. Верни только валидный JSON, ничего больше." }
        }
    };

    private Ritual? ParseRitualFromResponse(string responseBody, Archetype archetype)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var rawContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            // Strip markdown code fences if model wrapped JSON in ```json ... ```
            var jsonContent = StripCodeFences(rawContent);
            var dto = JsonSerializer.Deserialize<LlmRitualDto>(jsonContent, JsonOpts);
            return dto is null ? null : MapToRitual(dto, archetype);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse LLM response");
            return null;
        }
    }

    private static string StripCodeFences(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            var lastFence = trimmed.LastIndexOf("```");
            if (firstNewline > 0 && lastFence > firstNewline)
                return trimmed[(firstNewline + 1)..lastFence].Trim();
        }
        return trimmed;
    }

    private static Ritual MapToRitual(LlmRitualDto dto, Archetype archetype) => new()
    {
        Id = Guid.TryParse(dto.RitualId, out var id) ? id : Guid.NewGuid(),
        Archetype = archetype,
        Title = dto.Title,
        KeyMetaphor = dto.KeyMetaphor,
        IsStatic = false,
        Phases = dto.Phases.Select(p => new RitualPhase
        {
            Name = p.Name,
            DurationSec = p.DurationSec,
            Instructions = p.Instructions.Select(i => new RitualInstruction
            {
                Type = Enum.TryParse<InstructionType>(i.Type, true, out var t) ? t : InstructionType.Text,
                Value = i.Value,
                Pattern = i.Pattern,
                Description = i.Description
            }).ToList()
        }).ToList()
    };
}

using HelpYourself.Core.DTOs;
using HelpYourself.Core.Enums;
using HelpYourself.Core.Interfaces;
using HelpYourself.Core.Models;
using HelpYourself.Infrastructure.StaticData;
using Microsoft.Extensions.Logging;

namespace HelpYourself.Api.Services;

public sealed class RitualService : IRitualService
{
    private readonly ILlmClient _llm;
    private readonly IRitualRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<RitualService> _logger;

    public RitualService(
        ILlmClient llm,
        IRitualRepository repository,
        ICacheService cache,
        ILogger<RitualService> logger)
    {
        _llm = llm;
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<GenerateRitualResponse> GenerateAsync(GenerateRitualRequest request, CancellationToken ct = default)
    {
        var ritual = await _llm.GenerateRitualAsync(request.Archetype, request.Context, ct);

        if (ritual is null)
        {
            _logger.LogWarning("LLM unavailable, falling back to static ritual for {Archetype}", request.Archetype);
            ritual = StaticRituals.GetForArchetype(request.Archetype);
        }
        else
        {
            // Persist LLM-generated ritual asynchronously — don't block the response
            _ = PersistAndCacheAsync(ritual, request.Archetype, ct);
        }

        return GenerateRitualResponse.FromRitual(ritual);
    }

    public async Task RecordFeedbackAsync(Guid ritualId, FeedbackRequest feedback, CancellationToken ct = default)
    {
        var entity = new RitualFeedback
        {
            RitualId = ritualId,
            Rating = feedback.Rating
        };
        await _repository.SaveFeedbackAsync(entity, ct);
    }

    public async Task<GenerateRitualResponse> GetInstantRitualAsync(Archetype archetype, CancellationToken ct = default)
    {
        var cached = await _cache.GetCachedRitualAsync(archetype, ct);
        if (cached is not null) return GenerateRitualResponse.FromRitual(cached);

        var ritual = StaticRituals.GetForArchetype(archetype);
        return GenerateRitualResponse.FromRitual(ritual);
    }

    private async Task PersistAndCacheAsync(Ritual ritual, Archetype archetype, CancellationToken ct)
    {
        try
        {
            await _repository.SaveAsync(ritual, ct);
            await _cache.SetCachedRitualAsync(archetype, ritual, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist ritual {RitualId}", ritual.Id);
        }
    }
}

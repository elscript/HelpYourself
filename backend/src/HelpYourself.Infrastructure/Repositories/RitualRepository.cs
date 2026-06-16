using System.Text.Json;
using HelpYourself.Core.Enums;
using HelpYourself.Core.Interfaces;
using HelpYourself.Core.Models;
using HelpYourself.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpYourself.Infrastructure.Repositories;

public sealed class RitualRepository : IRitualRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<RitualRepository> _logger;

    public RitualRepository(AppDbContext db, ILogger<RitualRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SaveAsync(Ritual ritual, CancellationToken ct = default)
    {
        var entity = new RitualEntity
        {
            Id = ritual.Id,
            Archetype = (int)ritual.Archetype,
            Title = ritual.Title,
            KeyMetaphor = ritual.KeyMetaphor,
            IsStatic = ritual.IsStatic,
            PhasesJson = JsonSerializer.Serialize(ritual.Phases),
            CreatedAt = ritual.CreatedAt
        };

        _db.Rituals.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Ritual?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Rituals.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);
        return entity is null ? null : MapToDomain(entity);
    }

    public async Task SaveFeedbackAsync(RitualFeedback feedback, CancellationToken ct = default)
    {
        var entity = new RitualFeedbackEntity
        {
            Id = feedback.Id,
            RitualId = feedback.RitualId,
            Rating = (int)feedback.Rating,
            CreatedAt = feedback.CreatedAt
        };

        _db.Feedbacks.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    private static Ritual MapToDomain(RitualEntity e)
    {
        var phases = JsonSerializer.Deserialize<List<RitualPhase>>(e.PhasesJson) ?? [];
        return new Ritual
        {
            Id = e.Id,
            Archetype = (Archetype)e.Archetype,
            Title = e.Title,
            KeyMetaphor = e.KeyMetaphor,
            IsStatic = e.IsStatic,
            Phases = phases,
            CreatedAt = e.CreatedAt
        };
    }
}

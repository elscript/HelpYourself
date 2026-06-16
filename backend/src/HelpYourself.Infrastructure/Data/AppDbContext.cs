using System.Text.Json;
using HelpYourself.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpYourself.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RitualEntity> Rituals => Set<RitualEntity>();
    public DbSet<RitualFeedbackEntity> Feedbacks => Set<RitualFeedbackEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RitualEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PhasesJson).HasColumnName("phases_json").HasColumnType("jsonb");
        });

        modelBuilder.Entity<RitualFeedbackEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.RitualId);
        });
    }
}

// Flat persistence entities — domain models contain value objects not suitable for direct EF mapping
public sealed class RitualEntity
{
    public Guid Id { get; set; }
    public int Archetype { get; set; }
    public string Title { get; set; } = string.Empty;
    public string KeyMetaphor { get; set; } = string.Empty;
    public bool IsStatic { get; set; }
    public string PhasesJson { get; set; } = "[]";
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class RitualFeedbackEntity
{
    public Guid Id { get; set; }
    public Guid RitualId { get; set; }
    public int Rating { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

using Microsoft.EntityFrameworkCore;
using Nexora.Api.Models;

namespace Nexora.Api.Data;

public class NexoraDbContext(DbContextOptions<NexoraDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<AlertHistory> AlertHistories => Set<AlertHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SystemName, e.EventId }).IsUnique();
            entity.HasIndex(e => new { e.RecordId, e.EventType });
            entity.Property(e => e.SystemName).HasMaxLength(100);
            entity.Property(e => e.EventId).HasMaxLength(200);
            entity.Property(e => e.RecordId).HasMaxLength(200);
            entity.Property(e => e.EventType).HasMaxLength(100);
            // Compatibilidade com o banco nexora_db existente: a coluna Data é TEXT.
            // O conteúdo continua sendo JSON serializado em string e é validado na API.
            entity.Property(e => e.Data).HasColumnType("text");
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => new { a.RecordId, a.EventType })
                .IsUnique().HasFilter("\"Resolved\" = false");
            entity.Property(a => a.FinancialDifference).HasPrecision(18, 2);
            entity.HasMany(a => a.History).WithOne(h => h.Alert)
                .HasForeignKey(h => h.AlertId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlertHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasIndex(h => new { h.AlertId, h.CreatedAt });
        });
    }
}

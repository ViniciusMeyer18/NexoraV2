using Microsoft.EntityFrameworkCore;
using Nexora.Api.Data;
using Nexora.Api.DTOs;

namespace Nexora.Api.Services;

public sealed class SystemPerformanceService(NexoraDbContext db)
{
    private sealed record SystemMetric(string Sistema, int TotalEventos, int RegistrosEnvolvidos,
        int TotalAlertasEnvolvidos, int AlertasPendentes, decimal PercentualAlertas, string Classificacao);

    public async Task<List<object>> GetSystemFailuresAsync(CancellationToken ct = default)
    {
        var events = await db.Events.AsNoTracking().ToListAsync(ct);
        var alerts = await db.Alerts.AsNoTracking().ToListAsync(ct);
        return BuildMetrics(events, alerts).Cast<object>().ToList();
    }

    public async Task<SystemHealthSummaryDto> GetHealthSummaryAsync(CancellationToken ct = default)
    {
        var events = await db.Events.AsNoTracking().ToListAsync(ct);
        var alerts = await db.Alerts.AsNoTracking().ToListAsync(ct);
        var metrics = BuildMetrics(events, alerts);
        return new SystemHealthSummaryDto {
            TotalSistemas = metrics.Count,
            SistemasEstaveis = metrics.Count(x => x.Classificacao == "Estável"),
            SistemasEmAtencao = metrics.Count(x => x.Classificacao == "Atenção"),
            SistemasCriticos = metrics.Count(x => x.Classificacao == "Crítico")
        };
    }

    private static List<SystemMetric> BuildMetrics(
        List<Nexora.Api.Models.Event> events, List<Nexora.Api.Models.Alert> alerts) =>
        events.GroupBy(e => e.SystemName).Select(group =>
        {
            var groupKeys = group.Select(e => (e.RecordId, e.EventType)).Distinct().ToHashSet();
            var related = alerts.Where(a => groupKeys.Contains((a.RecordId, a.EventType))).ToList();
            // A taxa usa registros/tipos afetados, não a quantidade de alertas:
            // alertas reincidentes não podem fazer o percentual ultrapassar 100%.
            var affectedKeys = related.Select(a => (a.RecordId, a.EventType)).Distinct().Count();
            var rate = groupKeys.Count == 0 ? 0m : affectedKeys * 100m / groupKeys.Count;
            return new SystemMetric(group.Key, group.Count(),
                group.Select(e => e.RecordId).Distinct().Count(), related.Count,
                related.Count(a => !a.Resolved), Math.Round(rate, 2),
                rate >= 15m ? "Crítico" : rate >= 5m ? "Atenção" : "Estável");
        }).OrderByDescending(x => x.TotalAlertasEnvolvidos).ToList();
}

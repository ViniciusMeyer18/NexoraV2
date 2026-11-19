using Microsoft.EntityFrameworkCore;
using Nexora.Api.Data;
using Nexora.Api.DTOs;

namespace Nexora.Api.Services;

public sealed class AlertRecurrenceService(NexoraDbContext db)
{
    public async Task<List<AlertRecurrenceDto>> GetRecurrencesAsync(CancellationToken ct = default)
    {
        var alerts = await db.Alerts.AsNoTracking().ToListAsync(ct);
        return alerts.GroupBy(a => new { a.RecordId, a.EventType })
            .Where(g => g.Count() > 1)
            .Select(g => new AlertRecurrenceDto {
                RecordId = g.Key.RecordId, EventType = g.Key.EventType,
                TotalOcorrencias = g.Count(), OcorrenciasPendentes = g.Count(a => !a.Resolved),
                ImpactoFinanceiroAcumulado = g.Sum(a => a.FinancialDifference),
                Classificacao = g.Count() >= 5 ? "Crítica" : g.Count() >= 3 ? "Recorrente" : "Ocasional",
                PrimeiraOcorrencia = g.Min(a => a.CreatedAt), UltimaOcorrencia = g.Max(a => a.CreatedAt)
            }).OrderByDescending(x => x.TotalOcorrencias).ToList();
    }
}

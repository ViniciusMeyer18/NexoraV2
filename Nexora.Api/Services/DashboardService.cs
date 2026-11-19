using Microsoft.EntityFrameworkCore;
using Nexora.Api.Data;
using Nexora.Api.DTOs;

namespace Nexora.Api.Services;

public sealed class DashboardService(NexoraDbContext db)
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var alerts = db.Alerts.AsNoTracking();
        return new DashboardSummaryDto
        {
            TotalEventos = await db.Events.CountAsync(ct),
            TotalAlertas = await alerts.CountAsync(ct),
            AlertasPendentes = await alerts.CountAsync(a => !a.Resolved, ct),
            AlertasResolvidos = await alerts.CountAsync(a => a.Resolved, ct),
            ImpactoFinanceiro = await alerts.Where(a => !a.Resolved)
                .Select(a => (decimal?)a.FinancialDifference).SumAsync(ct) ?? 0m,
            TotalReincidencias = await alerts.GroupBy(a => new { a.RecordId, a.EventType })
                .CountAsync(g => g.Count() > 1, ct)
        };
    }
}

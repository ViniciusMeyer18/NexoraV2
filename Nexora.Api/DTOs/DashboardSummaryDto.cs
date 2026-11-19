namespace Nexora.Api.DTOs;

public class DashboardSummaryDto
{
    public int TotalEventos { get; set; }
    public int TotalAlertas { get; set; }
    public int AlertasPendentes { get; set; }
    public int AlertasResolvidos { get; set; }
    public decimal ImpactoFinanceiro { get; set; }
    public int TotalReincidencias { get; set; }
}

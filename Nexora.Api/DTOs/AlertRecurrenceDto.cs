namespace Nexora.Api.DTOs;

public class AlertRecurrenceDto
{
    public string RecordId { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public int TotalOcorrencias { get; set; }

    public int OcorrenciasPendentes { get; set; }

    public DateTime PrimeiraOcorrencia { get; set; }

    public DateTime UltimaOcorrencia { get; set; }
    
    public decimal ImpactoFinanceiroAcumulado { get; set; }
    
    public string Classificacao { get; set; } = string.Empty;
}
namespace Nexora.Api.DTOs;

public class SystemHealthSummaryDto
{
    public int TotalSistemas { get; set; }
    public int SistemasEstaveis { get; set; }
    public int SistemasEmAtencao { get; set; }
    public int SistemasCriticos { get; set; }
}

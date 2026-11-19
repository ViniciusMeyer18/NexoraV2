using Nexora.Api.Models;

namespace Nexora.Api.Services;

public sealed class AlertPriorityService
{
    public int CalcularPrioridade(Alert alerta)
    {
        var score = alerta.Severity switch {
            "Alta" => 60, "Média" => 35, _ => 15
        };
        score += alerta.FinancialDifference >= 1000 ? 40
            : alerta.FinancialDifference >= 500 ? 25
            : alerta.FinancialDifference >= 100 ? 15 : 0;
        return Math.Clamp(score, 0, 100);
    }

    public string ClassificarPrioridade(int pontuacao) => pontuacao switch {
        >= 80 => "Crítica", >= 60 => "Alta", >= 35 => "Média", _ => "Baixa"
    };
}

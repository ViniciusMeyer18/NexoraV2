namespace Nexora.Api.Models;

public class Alert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RecordId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal FinancialDifference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Resolved { get; set; }
    public ICollection<AlertHistory> History { get; set; } = new List<AlertHistory>();
}

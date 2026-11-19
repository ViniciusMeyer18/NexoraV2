namespace Nexora.Api.Models;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SystemName { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string Data { get; set; } = "{}";
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Nexora.Api.DTOs;

public class CreateEventRequest
{
    [Required, StringLength(100)]
    public string SystemName { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string EventId { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string RecordId { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string EventType { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public JsonElement Data { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Uptime.Stars.Contracts.Monitors;
public class MonitorRequest
{
    [Required]
    public string Name { get; set; } = "";

    public string? Description { get; set; } = "";

    public Guid? GroupId { get; set; }

    [Required]
    public int Type { get; set; }

    [Required]
    public string Target { get; set; } = "";

    [Required]
    public int IntervalInMinutes { get; set; } = 1;

    [Required]
    public int TiemoutInMilliseconds { get; set; } = 10000;

    public string[] RequestHeaders { get; set; } = [];
    
    public int? SearchMode { get; set; }
    
    public string? ExpectedText{ get; set; }

    public string[] AlertEmails { get; set; } = [];

    public string? AlertMessage { get; set; }

    [Required]
    public int AlertDelayMinutes { get; set; } = 0;

    [Required]
    public int AlertResendCycles { get; set; } = 1;
}
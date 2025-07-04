﻿using Uptime.Stars.Contracts.Events;

namespace Uptime.Stars.Contracts.Monitors;

public class MonitorResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid? GroupId { get; set; }
    public bool? IsUp { get; set; }
    public string Target { get; set; } = "";
    public string CreatedAtUtc { get; set; } = "";
    public bool IsActive { get; set; }
    public int IntervalInMinutes { get; set; }
    public int TiemoutInMilliseconds { get; set; }
    public int Type { get; set; }
    public string[] RequestHeaders { get; set; } = [];
    public int? SearchMode { get; set; }
    public string? ExpectedText { get; set; }
    public string[] AlertEmails { get; set; } = [];
    public string? AlertMessage { get; set; }
    public int AlertDelayMinutes { get; set; } = 0;
    public int AlertResendCycles { get;  set; } = 3;
    public IReadOnlyCollection<EventResponse>? LastEvents { get; set; }
    public string Uptime24hPercentage { get; set; } = "";
    public string Uptime30dPercentage { get; set; } = "";
}
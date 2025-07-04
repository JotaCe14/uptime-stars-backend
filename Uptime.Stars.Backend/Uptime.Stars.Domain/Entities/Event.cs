﻿using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Domain.Entities;
public class Event : AggregateRoot
{
    protected Event() { }
    private Event(
        Guid monitorId,
        DateTime timestampUtc,
        int nextCheckInMinutes,
        bool isUp = true,
        bool isImportant = false,
        string? message = null,
        long? latencyMilliseconds = null)
    {
        MonitorId = monitorId;
        IsUp = isUp;
        IsImportant = isImportant;
        NextCheckInMinutes = nextCheckInMinutes;
        Message = message;
        TimestampUtc = timestampUtc;
        LatencyMilliseconds = latencyMilliseconds;
    }
    public static Event Create(
        Guid MonitorId, 
        DateTime timestampUtc,
        int nextCheckInMinutes,
        bool isUp = true,
        bool isImportant = false,
        string? message = null,
        long? latencyMilliseconds = null)
    {
        return new Event(MonitorId, timestampUtc, nextCheckInMinutes, isUp, isImportant, message, latencyMilliseconds);
    }

    public void Update(
        Category? category,
        MaintenanceType? maintenanceType,
        bool falsePositive = false,
        string? note = "",
        string? ticketId = "")
    {
        FalsePositive = falsePositive;
        Category = category;
        Note = note;
        TicketId = ticketId;
        MaintenanceType = maintenanceType;
    }

    public Guid MonitorId { get; private set; }
    public ComponentMonitor Monitor { get; }
    public bool IsUp { get; private set; } = true;
    public bool IsImportant { get; private set; } = false;
    public int NextCheckInMinutes { get; private set; }
    public long? LatencyMilliseconds { get; private set; }
    public DateTime TimestampUtc { get; private set; } = DateTime.UtcNow;
    public string? Message { get; private set; }
    public bool FalsePositive { get; private set; } = false;
    public Category? Category { get; private set; }
    public string? Note { get; private set; } = "";
    public string? TicketId { get; private set; } = "";
    public MaintenanceType? MaintenanceType { get; set; }
}
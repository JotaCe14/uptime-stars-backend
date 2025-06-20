using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Domain.Entities;
public class ComponentMonitor : AggregateRoot
{
    protected ComponentMonitor() { }

    private ComponentMonitor(
        string name,
        string description,
        MonitorType type,
        string target,
        DateTime createdAt,
        string[]? requestHeaders,
        string[]? alertEmails,
        Guid? groupId = null,
        int intervalInMinutes = 1,
        int timeoutInMilliseconds = 10000,
        TextSearchMode? searchMode = null,
        string? expectedText = null,
        string? alertMessage = null,
        int alertDelayMinutes = 0,
        int alertResendCycles = 3)
    {
        Name = name;
        Description = description;
        GroupId = groupId;
        Type = type;
        Target = target;
        CreatedAt = createdAt;
        RequestHeaders = requestHeaders ?? [];
        AlertEmails = alertEmails ?? [];
        IntervalInMinutes = intervalInMinutes;
        TiemoutInMilliseconds = timeoutInMilliseconds;
        SearchMode = searchMode;
        ExpectedText = expectedText;
        AlertMessage = alertMessage;
        AlertDelayMinutes = alertDelayMinutes;
        AlertResendCycles = alertResendCycles;
        IsUp = true;
    }

    public static ComponentMonitor Create(
        string name,
        string description,
        MonitorType type,
        string target,
        DateTime createdAt,
        string[] requestHeaders,
        string[] alertEmails,
        Guid? groupId = null,
        int intervalInMinutes = 1,
        int timeoutInMilliseconds = 10000,
        TextSearchMode? searchMode = null,
        string? expectedText = null,
        string? alertMessage = "",
        int alertDelayMinutes = 0,
        int alertResendCycles = 3)
    {
        return new ComponentMonitor
        (
            name,
            description,
            type,
            target,
            createdAt,
            requestHeaders,
            alertEmails,
            groupId,
            intervalInMinutes,
            timeoutInMilliseconds,
            searchMode,
            expectedText,
            alertMessage,
            alertDelayMinutes,
            alertResendCycles
        );
    }

    public void Update(
        string name,
        string description,
        MonitorType type,
        string target,
        string[] requestHeaders,
        string[] alertEmails,
        Guid? groupId = null,
        int intervalInMinutes = 1,
        int timeoutInMilliseconds = 10000,
        TextSearchMode? searchMode = null,
        string? expectedText = null,
        string? alertMessage = "",
        int alertDelayMinutes = 0,
        int alertResendCycles = 3)
    {
        Name = name;
        Description = description;
        GroupId = groupId;
        Type = type;
        Target = target;
        RequestHeaders = requestHeaders;
        AlertEmails = alertEmails;
        IntervalInMinutes = intervalInMinutes;
        TiemoutInMilliseconds = timeoutInMilliseconds;
        SearchMode = searchMode;
        ExpectedText = expectedText;
        AlertMessage = alertMessage;
        AlertDelayMinutes = alertDelayMinutes;
        AlertResendCycles = alertResendCycles;
    }

    public void Disable()
    {
        IsActive = false;
    }
    public void Enable()
    {
        IsActive = true;
    }

    public void Check(bool isUp)
    {
        IsUp = isUp;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public MonitorType Type { get; private set; }
    public string Target { get; private set; }
    public int IntervalInMinutes { get; private set; } = 1;
    public int TiemoutInMilliseconds { get; private set; } = 10000;
    public string[] RequestHeaders { get; private set; } = [];
    public TextSearchMode? SearchMode { get; private set; }
    public string? ExpectedText { get; private set; }
    public string[] AlertEmails { get; private set; } = [];
    public string? AlertMessage { get; private set; }
    public int AlertDelayMinutes { get; private set; } = 0;
    public int AlertResendCycles { get; private set; } = 3;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsUp { get; set; } = true;
    public Guid? GroupId { get; private set; }
    public Group? Group { get; }
    public virtual ICollection<Event> Events { get; set; } = [];
}
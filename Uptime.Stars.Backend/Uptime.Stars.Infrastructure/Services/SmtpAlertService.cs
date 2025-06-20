using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class SmtpAlertService(IConfiguration configuration) : IAlertService
{
    public async Task SendAlertAsync(ComponentMonitor monitor, Event @event, CancellationToken cancellationToken = default)
    {
        var subject = @event.IsUp ? $"✅ {monitor.Name} is UP" : $"⚠️ {monitor.Name} is DOWN";

        var downMessage = string.IsNullOrWhiteSpace(monitor.AlertMessage) ? $"Monitor {monitor.Name} is DOWN: {@event.Message}" : monitor.AlertMessage;
        
        var body = @event.IsUp ? $"Monitor {monitor.Name} is UP again." : downMessage;

        var message = new MailMessage
        {
            Subject = subject,
            Body = body,
            From = new MailAddress("no-reply@uptime.starts")
        };

        if (monitor.AlertEmails.Length == 0)
        {
            return;
        }

        foreach (var to in monitor.AlertEmails)
            message.To.Add(to);

        using var client = new SmtpClient(configuration["MailSettings:Host"], Convert.ToInt32(configuration["MailSettings:Port"] ?? "25"));

        await client.SendMailAsync(message, cancellationToken);
    }
}
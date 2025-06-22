using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class SmtpAlertService(IConfiguration configuration, ISmtpClientWrapper smtpClientWrapper) : IAlertService
{
    public async Task SendAlertAsync(ComponentMonitor monitor, Event @event, CancellationToken cancellationToken = default)
    {
        if (monitor.AlertEmails.Length == 0)
        {
            return;
        }

        var subject = @event.IsUp ? $"✅ {monitor.Name} is UP" : $"⚠️ {monitor.Name} is DOWN";

        var downMessage = string.IsNullOrWhiteSpace(monitor.AlertMessage) ? $"Monitor {monitor.Name} is DOWN: {@event.Message}" : monitor.AlertMessage;
        
        var body = @event.IsUp ? $"Monitor {monitor.Name} is UP again." : downMessage;

        var message = new MailMessage
        {
            Subject = subject,
            Body = body,
            From = new MailAddress("no-reply@uptime.starts")
        };

        foreach (var to in monitor.AlertEmails)
            message.To.Add(to);

        await smtpClientWrapper.SendMailAsync(
            message, 
            configuration["MailSettings:Host"] ?? "", 
            Convert.ToInt32(configuration["MailSettings:Port"] ?? "25"), 
            cancellationToken);
    }
}

internal interface ISmtpClientWrapper
{
    Task SendMailAsync(MailMessage message, string host, int port, CancellationToken cancellationToken = default);
}

internal class SmtpClientWrapper : ISmtpClientWrapper
{
    public async Task SendMailAsync(MailMessage message, string host, int port, CancellationToken cancellationToken = default)
    {
        using var _client = new SmtpClient(host, port);

        await _client.SendMailAsync(message, cancellationToken);
    }
}
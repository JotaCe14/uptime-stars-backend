using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Net.Mail;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Infrastructure.Services;

namespace Uptime.Stars.UnitTests.Infrastructure.Services;

public class SmtpAlertServiceTests
{
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ISmtpClientWrapper _smtpClientWrapper = Substitute.For<ISmtpClientWrapper>();
    private readonly SmtpAlertService _service;

    public SmtpAlertServiceTests()
    {
        _configuration["MailSettings:Host"].Returns("smtp.test.com");
        _configuration["MailSettings:Port"].Returns("2525");
        _service = new SmtpAlertService(_configuration, _smtpClientWrapper);
    }

    private static ComponentMonitor GetMonitor(string[] emails, string? alertMessage = null)
    {
        return ComponentMonitor.Create(
            "TestMonitor",
            "desc",
            MonitorType.Https,
            "https://test.com",
            DateTime.UtcNow,
            alertEmails: emails,
            requestHeaders: [],
            timeoutInMilliseconds: 1000,
            alertMessage: alertMessage
        );
    }

    private static Event GetEvent(bool isUp, string? message = null)
    {
        return Event.Create(Guid.NewGuid(), DateTime.UtcNow, isUp, false, message ?? "test", 100);
    }

    [Fact]
    public async Task SendAlertAsync_DoesNothing_WhenNoAlertEmails()
    {
        // Arrange

        var monitor = GetMonitor([]);
        
        var @event = GetEvent(false);

        // Act
        
        await _service.SendAlertAsync(monitor, @event, CancellationToken.None);

        // Assert

        await _smtpClientWrapper.DidNotReceiveWithAnyArgs().SendMailAsync(Arg.Any<MailMessage>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAlertAsync_SendsMail_WithDownMessageAndDefaultBody()
    {
        // Arrange

        var monitor = GetMonitor(["to@test.com"]);
        var @event = GetEvent(false, "down msg");

        // Act
        
        await _service.SendAlertAsync(monitor, @event, CancellationToken.None);

        // Assert

        await _smtpClientWrapper.Received(1).SendMailAsync(
            Arg.Is<MailMessage>(m =>
                m.Subject.Contains("DOWN") &&
                m.Body.Contains("down msg") &&
                m.To.Contains(new MailAddress("to@test.com"))
            ),
            "smtp.test.com",
            2525,
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task SendAlertAsync_SendsMail_WithCustomDownMessage()
    {
        // Arrange

        var monitor = GetMonitor(["to@test.com"], "Custom DOWN!");
        var @event = GetEvent(false, "down msg");

        // Act
        
        await _service.SendAlertAsync(monitor, @event, CancellationToken.None);

        // Assert

        await _smtpClientWrapper.Received(1).SendMailAsync(
            Arg.Is<MailMessage>(m =>
                m.Body.Contains("Custom DOWN!")
            ),
            "smtp.test.com",
            2525,
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task SendAlertAsync_SendsMail_WithUpMessage()
    {
        // Arrange

        var monitor = GetMonitor(["to@test.com"]);
        var @event = GetEvent(true);

        // Act
        
        await _service.SendAlertAsync(monitor, @event, CancellationToken.None);

        // Assert

        await _smtpClientWrapper.Received(1).SendMailAsync(
            Arg.Is<MailMessage>(m =>
                m.Subject.Contains("UP") &&
                m.Body.Contains("UP again")
            ),
            "smtp.test.com",
            2525,
            Arg.Any<CancellationToken>()
        );
    }
}

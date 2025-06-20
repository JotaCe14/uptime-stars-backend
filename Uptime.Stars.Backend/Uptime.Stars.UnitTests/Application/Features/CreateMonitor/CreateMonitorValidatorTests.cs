using FluentValidation.TestHelper;
using Uptime.Stars.Application.Features.CreateMonitor;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.UnitTests.Application.Features.CreateMonitor;

public class CreateMonitorValidatorTests
{
    private readonly CreateMonitorValidator _validator = new();

    private static CreateMonitorCommand GetCommand()
    {
        return new CreateMonitorCommand(
            "Monitor1",
            "Description",
            Guid.NewGuid(),
            MonitorType.Ping,
            "127.0.0.1",
            1,
            1000,
            [],
            TextSearchMode.Include,
            "ok",
            [],
            "",
            0,
            0
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(60)]
    public void IntervalInMinutes_ValidValues_ShouldNotHaveValidationError(int interval)
    {
        // Arrange
        
        var command = GetCommand() with { IntervalInMinutes = interval };
        
        // Act
        
        var result = _validator.TestValidate(command);
        
        // Assert

        result.ShouldNotHaveValidationErrorFor(c => c.IntervalInMinutes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public void IntervalInMinutes_InvalidValues_ShouldHaveValidationError(int interval)
    {
        // Arrange

        var command = GetCommand() with { IntervalInMinutes = interval };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(c => c.IntervalInMinutes);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30000)]
    public void TimeoutInMilliseconds_ValidRange_ShouldNotHaveValidationError(int timeout)
    {
        // Arrange

        var command = GetCommand() with { TiemoutInMilliseconds = timeout };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveValidationErrorFor(c => c.TiemoutInMilliseconds);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(30001)]
    public void TimeoutInMilliseconds_InvalidRange_ShouldHaveValidationError(int timeout)
    {
        // Arrange

        var command = GetCommand() with { TiemoutInMilliseconds = timeout };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(c => c.TiemoutInMilliseconds);
    }

    [Fact]
    public void SearchMode_Required_WhenExpectedTextProvided()
    {
        // Arrange

        var command = GetCommand() with { ExpectedText = "test", SearchMode = null };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(c => c.SearchMode);
    }

    [Fact]
    public void SearchMode_NotRequired_WhenExpectedTextIsNull()
    {
        // Arrange

        var command = GetCommand() with { ExpectedText = null, SearchMode = null };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveValidationErrorFor(c => c.SearchMode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    public void AlertDelayMinutes_ValidRange_ShouldNotHaveValidationError(int value)
    {
        // Arrange

        var command = GetCommand() with { AlertDelayMinutes = value };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveValidationErrorFor(c => c.AlertDelayMinutes);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(61)]
    public void AlertDelayMinutes_InvalidRange_ShouldHaveValidationError(int value)
    {
        // Arrange

        var command = GetCommand() with { AlertDelayMinutes = value };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(c => c.AlertDelayMinutes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    public void AlertResendCycles_ValidRange_ShouldNotHaveValidationError(int value)
    {
        // Arrange

        var command = GetCommand() with { AlertResendCycles = value };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveValidationErrorFor(c => c.AlertResendCycles);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(61)]
    public void AlertResendCycles_InvalidRange_ShouldHaveValidationError(int value)
    {
        // Arrange

        var command = GetCommand() with { AlertResendCycles = value };

        // Act

        var result = _validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(c => c.AlertResendCycles);
    }
}
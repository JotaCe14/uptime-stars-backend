// C#
using FluentValidation.TestHelper;
using Uptime.Stars.Application.Features.GenerateMonitorReport;
using Uptime.Stars.Application.Features.GenerateReport;

namespace Uptime.Stars.UnitTests.Application.Features.GenerateMonitorReport;

public class GenerateMonitorReportValidatorTests
{
    private readonly GenerateMonitorReportValidator _validator = new();

    private static GenerateMonitorReportCommand GetCommand(string? dateFrom = "01/01/2024", string? dateTo = "02/01/2024")
    {
        return new GenerateMonitorReportCommand(dateFrom!, dateTo!);
    }

    [Theory]
    [InlineData("01/01/2024")]
    [InlineData("31/12/2023")]
    public void DateFrom_ValidFormat_ShouldNotHaveValidationError(string date)
    {
        var command = GetCommand(dateFrom: date);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.DateFrom);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2024-01-01")]
    [InlineData("32/01/2024")]
    [InlineData("01-01-2024")]
    public void DateFrom_InvalidFormat_ShouldHaveValidationError(string date)
    {
        var command = GetCommand(dateFrom: date);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.DateFrom);
    }

    [Theory]
    [InlineData("02/01/2024")]
    [InlineData("31/12/2024")]
    public void DateTo_ValidFormat_ShouldNotHaveValidationError(string dateTo)
    {
        var command = GetCommand(dateTo: dateTo);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.DateTo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2024-01-02")]
    [InlineData("32/01/2024")]
    [InlineData("02-01-2024")]
    public void DateTo_InvalidFormat_ShouldHaveValidationError(string dateTo)
    {
        var command = GetCommand(dateTo: dateTo);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.DateTo);
    }

    [Fact]
    public void DateTo_MustBeGreaterThanDateFrom_ShouldNotHaveValidationError()
    {
        var command = GetCommand(dateFrom: "01/01/2024", dateTo: "02/01/2024");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.DateTo);
    }

    [Fact]
    public void DateTo_EqualOrLessThanDateFrom_ShouldHaveValidationError()
    {
        var command1 = GetCommand(dateFrom: "02/01/2024", dateTo: "01/01/2024");
        var result1 = _validator.TestValidate(command1);
        result1.ShouldHaveValidationErrorFor(c => c.DateTo);

        var command2 = GetCommand(dateFrom: "01/01/2024", dateTo: "01/01/2024");
        var result2 = _validator.TestValidate(command2);
        result2.ShouldHaveValidationErrorFor(c => c.DateTo);
    }
}

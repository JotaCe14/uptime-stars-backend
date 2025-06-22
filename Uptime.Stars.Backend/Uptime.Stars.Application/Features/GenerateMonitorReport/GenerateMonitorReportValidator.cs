using FluentValidation;
using System.Globalization;
using Uptime.Stars.Application.Features.GenerateReport;

namespace Uptime.Stars.Application.Features.GenerateMonitorReport;
internal class GenerateMonitorReportValidator : AbstractValidator<GenerateMonitorReportCommand>
{
    public GenerateMonitorReportValidator()
    {
        RuleFor(command => command.DateFrom)
            .NotEmpty()
            .Must(BeAValidDate);

        RuleFor(command => command.DateTo)
            .NotEmpty()
            .Must(BeAValidDate)
            .Must((command, dateTo) => BeGreaterThanDateFrom(command.DateFrom, dateTo));
    }

    private static bool BeAValidDate(string date)
    {
        return DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    private static bool BeGreaterThanDateFrom(string dateFrom, string dateTo)
    {
        if (DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var from) && 
            DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var to))
        {
            return to > from;
        }
        return false;
    }
}

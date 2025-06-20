using FluentValidation;

namespace Uptime.Stars.Application.Features.UpdateMonitor;
internal sealed class UpdateMonitorValidator : AbstractValidator<UpdateMonitorCommand>
{
    public UpdateMonitorValidator()
    {
        RuleFor(command => command.IntervalInMinutes)
            .Must(BeAValidInterval)
            .WithMessage("IntervalInMinutes must be one of the following values: 1, 5, 15, 30, or 60");

        RuleFor(command => command.TiemoutInMilliseconds)
            .InclusiveBetween(10, 30000)
            .WithMessage("TimeoutInMilliseconds must be between 10 and 30000");

        RuleFor(command => command.SearchMode)
            .NotNull()
            .When(command => command.ExpectedText is not null)
            .WithMessage("TextSearchMode must not be null when ExpectedText is provided");

        RuleFor(command => command.AlertDelayMinutes)
            .InclusiveBetween(0, 60)
            .WithMessage("AlertDelayMinutes must be between 0 and 60");

        RuleFor(command => command.AlertResendCycles)
            .InclusiveBetween(0, 60)
            .WithMessage("AlertResendCycles must be between 0 and 60");
    }

    private bool BeAValidInterval(int interval)
    {
        return interval == 1 || interval == 5 || interval == 15 || interval == 30 || interval == 60;
    }
}
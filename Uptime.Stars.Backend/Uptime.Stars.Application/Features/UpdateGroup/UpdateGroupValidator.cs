using FluentValidation;

namespace Uptime.Stars.Application.Features.UpdateGroup;
internal sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty();
    }
}
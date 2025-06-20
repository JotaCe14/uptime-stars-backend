using FluentValidation;

namespace Uptime.Stars.Application.Features.CreateGroup;
internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty();
    }
}
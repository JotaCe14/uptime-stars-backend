using FluentValidation.TestHelper;
using Uptime.Stars.Application.Features.CreateGroup;

namespace Uptime.Stars.UnitTests.Application.Features.CreateGroup;

public class CreateGroupValidatorTests
{
    private readonly CreateGroupValidator _validator = new();

    private static CreateGroupCommand GetCommand()
    {
        return new CreateGroupCommand("Test Group", "Default Description");
    }

    [Fact]
    public void Name_NotEmpty_ShouldNotHaveValidationError()
    {
        // Arrange

        var command = GetCommand();

        // Act
        
        var result = _validator.TestValidate(command);

        // Assert
        
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Name_EmptyOrNull_ShouldHaveValidationError(string? name)
    {
        // Arrange

        var command = GetCommand() with { Name = name };

        // Act
        
        var result = _validator.TestValidate(command);

        // Assert
        
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }
}

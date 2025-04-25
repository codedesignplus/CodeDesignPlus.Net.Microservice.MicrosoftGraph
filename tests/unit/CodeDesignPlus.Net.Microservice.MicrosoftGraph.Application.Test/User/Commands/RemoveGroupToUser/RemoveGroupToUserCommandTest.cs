using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.RemoveGroupToUser;

public class RemoveGroupToUserCommandTest
{
    private readonly Validator validator;

    public RemoveGroupToUserCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new RemoveGroupToUserCommand(Guid.Empty, "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Role_Is_Empty()
    {
        // Arrange
        var command = new RemoveGroupToUserCommand(Guid.NewGuid(), string.Empty);

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new RemoveGroupToUserCommand(Guid.NewGuid(), "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

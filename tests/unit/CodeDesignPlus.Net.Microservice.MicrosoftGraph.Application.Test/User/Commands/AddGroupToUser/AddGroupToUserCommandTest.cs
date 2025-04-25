using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandTest
{
    private readonly Validator validator;

    public AddGroupToUserCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new AddGroupToUserCommand(Guid.Empty, "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Role_Is_Empty()
    {
        // Arrange
        var command = new AddGroupToUserCommand(Guid.NewGuid(), string.Empty);

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new AddGroupToUserCommand(Guid.NewGuid(), "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}

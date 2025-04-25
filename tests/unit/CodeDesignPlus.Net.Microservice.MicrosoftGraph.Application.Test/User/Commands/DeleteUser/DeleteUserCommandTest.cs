using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.DeleteUser;

public class DeleteUserCommandTest
{
    private readonly Validator validator;

    public DeleteUserCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.Empty);

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

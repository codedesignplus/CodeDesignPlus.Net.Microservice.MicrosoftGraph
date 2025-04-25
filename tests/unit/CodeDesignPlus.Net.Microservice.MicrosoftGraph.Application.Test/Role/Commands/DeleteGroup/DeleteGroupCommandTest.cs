using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.DeleteGroup;

public class DeleteGroupCommandTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var command = new DeleteGroupCommand(Guid.Empty);

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var validator = new Validator();
        var command = new DeleteGroupCommand(Guid.NewGuid());

        // Act & Assert
        validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

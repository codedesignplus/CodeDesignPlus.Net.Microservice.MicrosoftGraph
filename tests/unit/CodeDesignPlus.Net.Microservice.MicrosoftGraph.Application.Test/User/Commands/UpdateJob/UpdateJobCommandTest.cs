using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateJob;

public class UpdateJobCommandTest
{
    private readonly Validator validator;

    public UpdateJobCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new UpdateJobCommand(Guid.Empty, new JobInfo());

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Job_Is_Null()
    {
        // Arrange
        var command = new UpdateJobCommand(Guid.NewGuid(), null!);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Job);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateJobCommand(Guid.NewGuid(), new JobInfo());

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

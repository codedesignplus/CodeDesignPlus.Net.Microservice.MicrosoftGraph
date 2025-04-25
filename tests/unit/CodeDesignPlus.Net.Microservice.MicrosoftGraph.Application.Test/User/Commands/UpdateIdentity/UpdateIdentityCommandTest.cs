using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateIdentity;

public class UpdateIdentityCommandTest
{
    private readonly Validator validator;

    public UpdateIdentityCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateIdentityCommand(Guid.Empty, "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "", "Doe", "John Doe", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "John", "", "John Doe", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Phone_Is_Empty()
    {
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

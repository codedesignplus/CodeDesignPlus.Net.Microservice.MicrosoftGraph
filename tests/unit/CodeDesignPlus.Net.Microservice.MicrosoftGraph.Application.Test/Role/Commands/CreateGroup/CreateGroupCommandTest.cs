using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.CreateGroup;

public class CreateGroupCommandTest
{
    private readonly Validator validator;

    public CreateGroupCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new CreateGroupCommand(Guid.Empty, "Test Name", "Test Description", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Name_Is_Null_Or_Empty()
    {
        var command = new CreateGroupCommand(Guid.NewGuid(), null!, "Test Description", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);

        command = new CreateGroupCommand(Guid.NewGuid(), string.Empty, "Test Description", true);
        result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Description_Is_Null_Or_Empty()
    {
        var command = new CreateGroupCommand(Guid.NewGuid(), "Test Name", null!, true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);

        command = new CreateGroupCommand(Guid.NewGuid(), "Test Name", string.Empty, true);
        result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_IsActive_Is_Null()
    {
        var command = new CreateGroupCommand(Guid.NewGuid(), "Test Name", "Test Description", false);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.IsActive);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_For_Valid_Command()
    {
        var command = new CreateGroupCommand(Guid.NewGuid(), "Test Name", "Test Description", true);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

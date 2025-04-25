using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.UpdateGroup;

public class UpdateGroupCommandTest
{
    private readonly Validator validator;

    public UpdateGroupCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateGroupCommand(Guid.Empty, "TestName", "TestDescription", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Name_Is_Null_Or_Empty()
    {
        var commandWithNullName = new UpdateGroupCommand(Guid.NewGuid(), null!, "TestDescription", true);
        var resultWithNullName = validator.TestValidate(commandWithNullName);
        resultWithNullName.ShouldHaveValidationErrorFor(x => x.Name);

        var commandWithEmptyName = new UpdateGroupCommand(Guid.NewGuid(), string.Empty, "TestDescription", true);
        var resultWithEmptyName = validator.TestValidate(commandWithEmptyName);
        resultWithEmptyName.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Description_Is_Null_Or_Empty()
    {
        var commandWithNullDescription = new UpdateGroupCommand(Guid.NewGuid(), "TestName", null!, true);
        var resultWithNullDescription = validator.TestValidate(commandWithNullDescription);
        resultWithNullDescription.ShouldHaveValidationErrorFor(x => x.Description);

        var commandWithEmptyDescription = new UpdateGroupCommand(Guid.NewGuid(), "TestName", string.Empty, true);
        var resultWithEmptyDescription = validator.TestValidate(commandWithEmptyDescription);
        resultWithEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateGroupCommand(Guid.NewGuid(), "ValidName", "ValidDescription", true);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateProfile;

public class UpdateProfileCommandTest
{
    private readonly Validator validator;

    public UpdateProfileCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateProfileCommand(Guid.Empty, "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_FirstName_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), null!, "Doe", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);

        command = new UpdateProfileCommand(Guid.NewGuid(), "", "Doe", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_LastName_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", null!, "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);

        command = new UpdateProfileCommand(Guid.NewGuid(), "John", "", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "invalid-email", "1234567890", new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Phone_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", null!, new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);

        command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "", new ContactInfo(), new JobInfo(), true);
        result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Contact_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", null!, new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Contact);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Job_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), null!, true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Job);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "John Doe", "john.doe@example.com", "1234567890", new ContactInfo(), new JobInfo(), true);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

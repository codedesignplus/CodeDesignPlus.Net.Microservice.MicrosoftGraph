using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateContactInfo;

public class UpdateContactInfoCommandTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var command = new UpdateContactInfoCommand(Guid.Empty, new ContactInfo());

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Contact_Is_Null()
    {
        // Arrange
        var validator = new Validator();
        var command = new UpdateContactInfoCommand(Guid.NewGuid(), null!);

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Contact);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var validator = new Validator();
        var contactInfo = new ContactInfo(); // Populate with valid data if necessary
        var command = new UpdateContactInfoCommand(Guid.NewGuid(), contactInfo);

        // Act & Assert
        validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

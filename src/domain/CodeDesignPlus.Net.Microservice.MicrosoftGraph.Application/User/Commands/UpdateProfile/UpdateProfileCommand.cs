using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;

[DtoGenerator]
public record UpdateProfileCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone, ContactInfo Contact, JobInfo Job, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateProfileCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}

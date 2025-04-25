using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;

[DtoGenerator]
public record UpdateProfileCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone, ContactInfo Contact, JobInfo Job, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateProfileCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
        RuleFor(x => x.Contact).NotEmpty().NotNull();
        RuleFor(x => x.Job).NotEmpty().NotNull();
    }
}

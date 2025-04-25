using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;

[DtoGenerator]
public record UpdateContactInfoCommand(Guid Id, ContactInfo Contact) : IRequest;

public class Validator : AbstractValidator<UpdateContactInfoCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Contact).NotEmpty().NotNull();
    }
}

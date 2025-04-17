namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;

[DtoGenerator]
public record UpdateIdentityCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
    }
}

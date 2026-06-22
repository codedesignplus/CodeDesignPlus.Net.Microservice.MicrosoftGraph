namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CompleteProviderRegistration;

public record CompleteProviderRegistrationCommand(string Email, Guid IdentityProviderId) : IRequest<Guid>;

public class Validator : AbstractValidator<CompleteProviderRegistrationCommand>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.IdentityProviderId).NotEmpty().NotNull();
    }
}

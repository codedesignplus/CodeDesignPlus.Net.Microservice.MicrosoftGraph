namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;

[DtoGenerator]
public record CreateUserFromSSOCommand(string FirstName, string LastName, string Email, string Phone, string? DisplayName, string DocumentNumber, bool IsActive) : IRequest<Guid>;

public class Validator : AbstractValidator<CreateUserFromSSOCommand>
{
    public Validator()
    {
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
        RuleFor(x => x.DocumentNumber).NotEmpty().NotNull();
    }
}

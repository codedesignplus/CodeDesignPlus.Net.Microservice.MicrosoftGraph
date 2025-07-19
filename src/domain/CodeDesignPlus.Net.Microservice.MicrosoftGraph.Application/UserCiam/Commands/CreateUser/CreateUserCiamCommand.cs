namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.CreateUser;

[DtoGenerator]
public record CreateUserCiamCommand(string FirstName, string LastName, string Email, string Phone, string? DisplayName, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateUserCiamCommand>
{
    public Validator()
    {
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
    }   
}

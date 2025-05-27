namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

[DtoGenerator]
public record CreateUserCommand(string FirstName, string LastName, string Email, string Phone, string? DisplayName, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
    }   
}

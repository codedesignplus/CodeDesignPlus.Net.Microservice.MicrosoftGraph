namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

[DtoGenerator]
public record CreateUserCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<CreateUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
